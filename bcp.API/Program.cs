using Bcp.API.Extensions;
using Bcp.API.Hubs;
using Bcp.API.Services;
using Bcp.Application;
using Bcp.Application.Contracts;
using Bcp.Infrastructure;
using Bcp.Infrastructure.Configuration;
using Microsoft.OpenApi.Models;
using System.Reflection;

var connectionString = ConnectionStringBuilder.BuildFromEnvironment();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(connectionString);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API V1", Version = "v1" });
});

// CORS for SignalR and API endpoints accessed from the Web UI
var containerUi = Environment.GetEnvironmentVariable("WEB_BASE_URL")
                 ?? Environment.GetEnvironmentVariable("UI_BASE_URL")
                 ?? "http://web:5000";
var hostUi = Environment.GetEnvironmentVariable("HOST_WEB_BASE_URL") ?? "http://localhost:5000";

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins(containerUi, hostUi)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()));

builder.Services.AddSignalR();

// Register notification publisher
builder.Services.AddSingleton<INotificationPublisher, NotificationPublisher>();

var app = builder.Build();
app.ApplyMigrations();
_ = app.UseSwagger();
_ = app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
    c.RoutePrefix = "swagger";
});

_ = app.UseCors();

_ = app.UseHttpsRedirection();
_ = app.MapControllers();
app.MapHub<NotificationsHub>("/hubs/notifications");
app.Run();