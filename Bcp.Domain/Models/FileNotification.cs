using Bcp.Domain.Enums;

namespace Bcp.Domain.Models;

/// <summary>
/// Represents a notification about a file that was uploaded.
/// </summary>
public class FileNotification
{
    /// <summary>
    /// Primary key for the FileNotification record.
    /// </summary>
    public int FileNotificationId { get; set; }

    /// <summary>
    /// The name under which the file was uploaded.
    /// </summary>
    public required string FileName { get; set; }

    /// <summary>
    /// Date and time the file was uploaded.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Indicates if the notification was processed.
    /// </summary>
    public NotificationStatus Status { get; set; }
}