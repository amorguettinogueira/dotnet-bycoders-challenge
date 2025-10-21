using Bcp.Application.DTOs;
using Bcp.Worker.Contracts;
using Bcp.Worker.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Refit;
using Xunit;

namespace Bcp.Worker.Tests;

public class HttpNotificationPublisherTests
{
    [Fact]
    public async Task Publish_Success_Logs_Info()
    {
        var api = new Mock<INotificationsApi>();
        _ = api.Setup(a => a.FileProcessedAsync(It.IsAny<FileProcessed>()))
           .ReturnsAsync(new ApiResponse<object>(new HttpResponseMessage(System.Net.HttpStatusCode.OK), new object(), null!));
        var logger = new Mock<ILogger<HttpNotificationPublisher>>();
        var sut = new HttpNotificationPublisher(api.Object, logger.Object);

        await sut.PublishFileProcessedAsync(1, "a.txt");

        logger.Verify(l => l.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((o, _) => o.ToString()!.Contains("Notification published successfully")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task Publish_Failure_Logs_Warning()
    {
        var api = new Mock<INotificationsApi>();
        var bad = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest) { Content = new StringContent("bad") };
        _ = api.Setup(a => a.FileProcessedAsync(It.IsAny<FileProcessed>()))
           .ReturnsAsync(new ApiResponse<object>(bad, null, null!));
        var logger = new Mock<ILogger<HttpNotificationPublisher>>();
        var sut = new HttpNotificationPublisher(api.Object, logger.Object);

        await sut.PublishFileProcessedAsync(1, "a.txt");

        logger.Verify(l => l.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((o, _) => o.ToString()!.Contains("Notification publish failed")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task Publish_ApiException_Logs_Error()
    {
        var api = new Mock<INotificationsApi>();
        var bad = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
        var apiEx = await ApiException.Create(new HttpRequestMessage(HttpMethod.Post, "http://t"), HttpMethod.Post, bad, new RefitSettings());
        _ = api.Setup(a => a.FileProcessedAsync(It.IsAny<FileProcessed>())).ThrowsAsync(apiEx);
        var logger = new Mock<ILogger<HttpNotificationPublisher>>();
        var sut = new HttpNotificationPublisher(api.Object, logger.Object);

        await sut.PublishFileProcessedAsync(1, "a.txt");

        logger.Verify(l => l.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((o, _) => o.ToString()!.Contains("API exception while publishing")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.AtLeastOnce);
    }
}