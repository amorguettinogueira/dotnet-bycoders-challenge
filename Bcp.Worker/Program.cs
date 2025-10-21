using Bcp.Application.Contracts;
using Bcp.Infrastructure;
using Bcp.Infrastructure.Configuration;
using Bcp.Worker.Contracts;
using Bcp.Worker.Services;
using Refit;

var connectionString = ConnectionStringBuilder.BuildFromEnvironment();

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddInfrastructure(connectionString);

var apiBase = Environment.GetEnvironmentVariable("API_BASE_URL") ?? "http://api:5163/";

// Refit client for notifications API with standard resilience policies
builder.Services.AddRefitClient<INotificationsApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBase))
    .AddStandardResilienceHandler();

// Adapter for application contract
builder.Services.AddSingleton<INotificationPublisher, HttpNotificationPublisher>();

builder.Services.AddHostedService<FileProcessingWorker>();

var host = builder.Build();
host.Run();
