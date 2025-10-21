using Bcp.Application.Contracts;
using Bcp.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Bcp.API.Controllers;

/// <summary>
/// Notifications endpoints used to publish events to connected clients (SignalR).
/// This controller exposes endpoints that other services (for example the worker)
/// can call to notify the API that a file was processed. The API will then broadcast
/// the event to browser clients via a SignalR hub.
/// </summary>
[ApiController]
[Route("api/notifications")]
public class NotificationsController(INotificationPublisher publisher) : ControllerBase
{
    /// <summary>
    /// Notify the API that a file has been processed.
    /// The API will broadcast the "FileProcessed" event to connected SignalR clients.
    /// </summary>
    /// <param name="dto">Payload containing the processed file id and file name.</param>
    /// <returns>200 OK when the notification was accepted.</returns>
    [HttpPost("file-processed")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> FileProcessed([FromBody] FileProcessed dto)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.FileName))
        {
            return BadRequest("Invalid payload");
        }

        await publisher.PublishFileProcessedAsync(dto.FileId, dto.FileName);
        return Ok();
    }
}