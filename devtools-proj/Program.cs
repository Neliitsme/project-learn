using devtools_proj.Persistence;
using devtools_proj.Services;
using devtools_proj.Settings;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Set up appsettings configs
builder.Services.Configure<MongoSettings>(builder.Configuration.GetSection("ConnectionStrings"));
builder.Services.AddSingleton<IMongoSettings>(sp => sp.GetRequiredService<IOptions<MongoSettings>>().Value);

// Set up mongo client, should be a singleton
builder.Services.AddSingleton<IMongoClient>(sp =>
    new MongoClient(builder.Configuration.GetConnectionString("ConnectionUri")));

// Set up made up db context 
builder.Services.AddScoped<IDbContext, DbContext>();

// Set up services
builder.Services.AddScoped<ISearchService, SearchService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Devtools proj API", Version = "v1" });
});

var app = builder.Build();

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