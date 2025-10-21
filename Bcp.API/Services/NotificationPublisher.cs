using Bcp.API.Hubs;
using Bcp.Application.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace Bcp.API.Services;

/// <summary>
/// API adapter that publishes "file processed" notifications to connected browser clients via SignalR.
/// This service implements the application contract <see cref="INotificationPublisher"/> and is registered
/// in the API DI container so other API components can use it to broadcast events.
/// </summary>
public class NotificationPublisher(IHubContext<NotificationsHub> hubContext) : INotificationPublisher
{
    /// <summary>
    /// Publishes a "FileProcessed" event to all connected SignalR clients.
    /// </summary>
    /// <param name="fileId">Identifier of the file entity that was processed.</param>
    /// <param name="fileName">Original file name (filename.ext).</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task PublishFileProcessedAsync(int fileId, string fileName) =>
        await hubContext.Clients.All.SendAsync("FileProcessed", new { FileId = fileId, FileName = fileName });
}
