namespace Bcp.Domain.Enums;

/// <summary>
/// Defines the current status of a file import notification.
/// </summary>
public enum NotificationStatus
{
    /// <summary>
    /// Added to the database waiting to be processed.
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Being processed by the worker.
    /// </summary>
    Processing = 2,

    /// <summary>
    /// Worked already resolved the notification.
    /// </summary>
    Completed = 3,
}