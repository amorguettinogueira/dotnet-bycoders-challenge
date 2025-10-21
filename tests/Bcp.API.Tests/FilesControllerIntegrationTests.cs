using Bcp.Application.Contracts;
using Bcp.Application.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Net.Http.Json;
using Xunit;

namespace Bcp.API.Tests;

public class FilesControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public FilesControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        // Ensure API boots without real DB in tests
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
        Environment.SetEnvironmentVariable("POSTGRES_DB", "test");
        Environment.SetEnvironmentVariable("POSTGRES_USER", "test");
        Environment.SetEnvironmentVariable("POSTGRES_PASSWORD", "test");

        _factory = factory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services =>
            {
                var svc = services.SingleOrDefault(d => d.ServiceType == typeof(ITransactionFileService));
                if (svc != null)
                {
                    _ = services.Remove(svc);
                }

                var mock = new Mock<ITransactionFileService>();
                _ = mock.Setup(m => m.GetTransactionsAsync(1, 2))
                    .ReturnsAsync(
                    [
                        new() { TransactionType = "Credit", Date = new DateOnly(2024,10,20), Time = new TimeSpan(10,0,0), Value = 100m },
                        new() { TransactionType = "Boleto", Date = new DateOnly(2024,10,21), Time = new TimeSpan(9,30,0), Value = -50m },
                    ]);
                _ = services.AddScoped(_ => mock.Object);

                var notif = services.SingleOrDefault(d => d.ServiceType == typeof(IFileNotificationService));
                if (notif != null)
                {
                    _ = services.Remove(notif);
                }

                _ = services.AddScoped<IFileNotificationService>(_ => Mock.Of<IFileNotificationService>());
            }));
    }

    [Fact]
    public async Task GetTransactions_Returns_Sorted_Items()
    {
        var client = _factory.CreateClient();

        var resp = await client.GetAsync("/api/files/1/stores/2/transactions");
        _ = resp.EnsureSuccessStatusCode();

        var items = await resp.Content.ReadFromJsonAsync<List<TransactionItem>>();
        Assert.NotNull(items);
        Assert.Equal(2, items!.Count);
        Assert.Equal("Credit", items[0].TransactionType);
        Assert.Equal("Boleto", items[1].TransactionType);
    }
}