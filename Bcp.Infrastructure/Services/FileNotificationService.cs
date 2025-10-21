using Bcp.Application.Contracts;
using Bcp.Domain.Models;
using Bcp.Infrastructure.Persistence;

namespace Bcp.Infrastructure.Services;

public class FileNotificationService : IFileNotificationService
{
    private readonly AppDbContext _db;

    public FileNotificationService(AppDbContext db)
    {
        _db = db;
    }

    public async Task NotifyAsync(string filePath)
    {
        _ = _db.FileNotifications.Add(new FileNotification
        {
            // Store only the file name; path will be resolved by the consumer (worker)
            FileName = Path.GetFileName(filePath),
            CreatedAt = DateTime.UtcNow,
            Status = Domain.Enums.NotificationStatus.Pending
        });

        await _db.SaveChangesAsync();
    }
}
