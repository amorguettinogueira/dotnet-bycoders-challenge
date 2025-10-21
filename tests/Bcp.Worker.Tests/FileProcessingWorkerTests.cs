using Bcp.Application.Interfaces;
using Bcp.Domain.Enums;
using Bcp.Domain.Models;
using Bcp.Infrastructure.Persistence;
using Bcp.Worker.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Bcp.Worker.Tests;

public class FileProcessingWorkerTests
{
    private static AppDbContext NewDb()
    {
        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(opts);
    }

    [Fact]
    public async Task ExecuteAsync_Picks_Pending_And_Calls_Processor()
    {
        await using var db = NewDb();
        _ = db.FileNotifications.Add(new FileNotification { FileName = "a.txt", CreatedAt = DateTime.UtcNow, Status = NotificationStatus.Pending });
        _ = await db.SaveChangesAsync();

        var logger = new Mock<ILogger<FileProcessingWorker>>();
        var processor = new Mock<ICnabFileProcessor>();
        _ = processor.Setup(p => p.ProcessAsync(It.IsAny<FileNotification>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var worker = new FileProcessingWorker(logger.Object, processor.Object, db);

        using var cts = new CancellationTokenSource();
        cts.CancelAfter(1500); // let loop run briefly
        await worker.StartAsync(cts.Token);

        // Give a moment for the background loop to tick once
        await Task.Delay(500);

        // Stop worker
        await worker.StopAsync(CancellationToken.None);

        processor.Verify(p => p.ProcessAsync(It.Is<FileNotification>(f => f.FileName == "a.txt"), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }
}