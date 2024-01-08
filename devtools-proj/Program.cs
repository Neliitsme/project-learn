using devtools_proj.Metrics.ReporterInterfaces;
using devtools_proj.Metrics.Reporters;
using devtools_proj.Persistence;
using devtools_proj.Services;
using devtools_proj.Settings;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Prometheus;
using Serilog;
using Serilog.Debugging;
using Serilog.Sinks.Grafana.Loki;

// TODO: Could move serilog configuration somewhere else
// https://github.com/serilog/serilog-aspnetcore?tab=readme-ov-file#two-stage-initialization
// https://github.com/serilog-contrib/serilog-sinks-grafana-loki/blob/master/sample/Serilog.Sinks.Grafana.Loki.SampleWebApp/appsettings.json

// TODO: https://grafana.com/blog/2020/04/21/how-labels-in-loki-can-make-log-queries-faster-and-easier/#cardinality

// Bootstrap Serilog for logging
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

SelfLog.Enable(Console.Error);

try
{
    var builder = WebApplication.CreateBuilder(args);

    Log.Information("Starting web application");

    // Set up appsettings configs
    Log.Information("Reading appsettings");
    builder.Services.AddOptions<ConnectionStrings>()
        .Bind(builder.Configuration.GetSection(nameof(ConnectionStrings)))
        .ValidateDataAnnotations();
    builder.Services.AddOptions<GeneralSettings>()
        .Bind(builder.Configuration.GetSection(nameof(GeneralSettings)))
        .ValidateDataAnnotations();

    var generalSettings = builder.Configuration.GetSection(nameof(GeneralSettings)).Get<GeneralSettings>() ??
                          throw new ArgumentNullException(nameof(GeneralSettings));
    var connectionStrings = builder.Configuration.GetSection(nameof(ConnectionStrings)).Get<ConnectionStrings>() ??
                            throw new ArgumentNullException(nameof(ConnectionStrings));

    // Set up appsettings configs for injecting 
    builder.Services.AddSingleton<IConnectionStrings>(sp => sp.GetRequiredService<IOptions<ConnectionStrings>>().Value);
    builder.Services.AddSingleton<IGeneralSettings>(sp => sp.GetRequiredService<IOptions<GeneralSettings>>().Value);

    // Add OTEL for traces
    builder.Services.AddOpenTelemetry()
        .ConfigureResource(r => r.AddService(generalSettings.ProjectName))
        .WithTracing(t => t.AddAspNetCoreInstrumentation().AddConsoleExporter().AddOtlpExporter().AddOtlpExporter(
            options => { options.Endpoint = new Uri("http://grafana-tempo:4317"); }));

    // Final Serilog setup
    Log.Information("Registering Serilog loki sink");
    builder.Host.UseSerilog((_, _, configuration) => configuration
        .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Application", generalSettings.ProjectName)
        .WriteTo.Console()
        .WriteTo.GrafanaLoki(connectionStrings.LokiUri, propertiesAsLabels: new[] { "Application" }));

    // Set up mongo client, should be a singleton
    Log.Information("Connecting to Mongo");
    var mongoClient = new MongoClient(connectionStrings.MongoUri);

    Log.Information("Verifying Mongo connection");
    try
    {
        mongoClient.GetDatabase("admin").RunCommand<BsonDocument>(new BsonDocument("ping", 1));
        Log.Information("Connected to mongo successfully");
    }
    catch (Exception)
    {
        Log.Fatal("Could not connect to the MongoDB");
        throw;
    }

    Log.Information("Registering DI services");

    builder.Services.AddSingleton<IMongoClient>(_ =>
        mongoClient);

    // Set up made up db context 
    builder.Services.AddSingleton<IDbContext, DbContext>();

    // Set up services
    builder.Services.AddScoped<ISearchService, SearchService>();

    // Set up metrics
    builder.Services.AddSingleton<ITracksMetricsReporter, TracksMetricsReporter>();

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo { Title = "Devtools proj API", Version = "v1" });
    });

    Log.Information("Building WebApp");
    var app = builder.Build();

    // Enable Serilog request logging
    app.UseSerilogRequestLogging();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseRouting();
    app.UseHttpMetrics();

    app.UseAuthorization();

    app.MapControllers();
    app.MapMetrics();

    Log.Information("Running WebApp");
    app.Run();
}
catch (Exception e)
{
    Log.Fatal(e, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}