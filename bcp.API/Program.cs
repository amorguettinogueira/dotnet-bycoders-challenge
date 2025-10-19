using bcp.API.Extensions;
using bcp.Application;
using bcp.Infrastructure;
using bcp.Infrastructure.Configuration;
using DotNetEnv;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("POSTGRES_DB")))
{
    Env.Load(Path.Combine(Directory.GetCurrentDirectory(), "..", ".env"));
}

var connectionString = ConnectionStringBuilder.BuildFromEnvironment();

//builder.Services.AddSingleton(appSettings);

builder.Services.AddControllers();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(connectionString);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API V1", Version = "v1" });
});

var app = builder.Build();
app.ApplyMigrations();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
    c.RoutePrefix = "swagger";
});
app.UseHttpsRedirection();
app.MapControllers();
app.Run();

