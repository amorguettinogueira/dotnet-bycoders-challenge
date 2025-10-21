using Bcp.API.Hubs;
using Bcp.API.Services;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Xunit;

namespace Bcp.API.Tests;

public class NotificationPublisherTests
{
    [Fact]
    public async Task PublishFileProcessedAsync_Sends_Message_To_All_Clients()
    {
        // Arrange
        var mockClients = new Mock<IHubClients>();
        var mockAll = new Mock<IClientProxy>();
        _ = mockClients.Setup(c => c.All).Returns(mockAll.Object);

        var mockContext = new Mock<IHubContext<NotificationsHub>>();
        _ = mockContext.Setup(c => c.Clients).Returns(mockClients.Object);

        var sut = new NotificationPublisher(mockContext.Object);

        // Act
        await sut.PublishFileProcessedAsync(123, "file.txt");

        // Assert
        mockAll.Verify(p => p.SendCoreAsync(
            "FileProcessed",
            It.Is<object[]>(args =>
                args.Length == 1 &&
                args[0]!.GetType().GetProperty("FileId")!.GetValue(args[0])!.Equals(123) &&
                args[0]!.GetType().GetProperty("FileName")!.GetValue(args[0])!.Equals("file.txt")
            ),
            default
        ), Times.Once);
    }
}