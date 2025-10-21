using Bcp.API.Controllers;
using Bcp.Application.Contracts;
using Bcp.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Bcp.API.Tests;

public class NotificationsControllerTests
{
    [Fact]
    public async Task FileProcessed_Returns_BadRequest_When_Payload_Invalid()
    {
        var publisher = new Mock<INotificationPublisher>();
        var controller = new NotificationsController(publisher.Object);

        var result = await controller.FileProcessed(new FileProcessed(0, ""));

        _ = Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task FileProcessed_Publishes_And_Returns_Ok()
    {
        var publisher = new Mock<INotificationPublisher>();
        var controller = new NotificationsController(publisher.Object);

        var dto = new FileProcessed(42, "data.txt");

        var result = await controller.FileProcessed(dto);

        _ = Assert.IsType<OkResult>(result);
        publisher.Verify(p => p.PublishFileProcessedAsync(42, "data.txt"), Times.Once);
    }
}