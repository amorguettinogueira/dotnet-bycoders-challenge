using Bcp.Infrastructure.Persistence;
using Bcp.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Bcp.Infrastructure.Tests;

public class FileNotificationServiceTests
{
    private static AppDbContext NewDb()
    {
        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(opts);
    }

    [Fact]
    public async Task NotifyAsync_Adds_Pending_Record_With_FileNameOnly()
    {
        await using var db = NewDb();
        var sut = new FileNotificationService(db);

        await sut.NotifyAsync("/path/sub/file.txt");

        var rec = db.FileNotifications.Single();
        Assert.Equal("file.txt", rec.FileName);
        Assert.True(rec.CreatedAt <= DateTime.UtcNow);
        Assert.Equal(Bcp.Domain.Enums.NotificationStatus.Pending, rec.Status);
    }
}
