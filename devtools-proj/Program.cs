using System.Diagnostics;
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
using Serilog.Sinks.Grafana.Loki;

// TODO: Could move this configuration somewhere else
// https://github.com/serilog/serilog-aspnetcore?tab=readme-ov-file#two-stage-initialization
// https://github.com/serilog-contrib/serilog-sinks-grafana-loki/blob/master/sample/Serilog.Sinks.Grafana.Loki.SampleWebApp/appsettings.json

// TODO: https://grafana.com/blog/2020/04/21/how-labels-in-loki-can-make-log-queries-faster-and-easier/#cardinality

const string appName = "devtools-proj";

// Set up logger
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", appName)
    .WriteTo.Console()
    .WriteTo.GrafanaLoki("http://localhost:3100", propertiesAsLabels: new[] { "Application" })
    .CreateLogger();

Serilog.Debugging.SelfLog.Enable(Console.Error);

try
{
    var builder = WebApplication.CreateBuilder(args);

    Log.Information("Starting web application");

    // Use Serilog for logging
    builder.Host.UseSerilog();

    // Add OTEL for traces
    builder.Services.AddOpenTelemetry()
        .ConfigureResource(r => r.AddService(appName))
        .WithTracing(t => t.AddAspNetCoreInstrumentation().AddConsoleExporter().AddOtlpExporter());

    // Set up appsettings configs
    builder.Services.Configure<MongoSettings>(builder.Configuration.GetSection("ConnectionStrings"));
    builder.Services.AddSingleton<IMongoSettings>(sp => sp.GetRequiredService<IOptions<MongoSettings>>().Value);

    // Set up mongo client, should be a singleton
    var mongoClient = new MongoClient(builder.Configuration.GetConnectionString("ConnectionUri"));
    try
    {
        mongoClient.GetDatabase("admin").RunCommand<BsonDocument>(new BsonDocument("ping", 1));
    }
    catch (Exception)
    {
        Log.Fatal("Could not connect to the MongoDB");
        throw;
    }

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