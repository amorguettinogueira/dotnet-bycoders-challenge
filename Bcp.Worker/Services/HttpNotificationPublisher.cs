using Bcp.Application.Contracts;
using Bcp.Application.DTOs;
using Bcp.Worker.Contracts;
using Refit;

namespace Bcp.Worker.Services;

public class HttpNotificationPublisher : INotificationPublisher
{
    private readonly INotificationsApi _api;
    private readonly ILogger<HttpNotificationPublisher> _logger;

    public HttpNotificationPublisher(INotificationsApi api, ILogger<HttpNotificationPublisher> logger)
    {
        _api = api;
        _logger = logger;
    }

    public async Task PublishFileProcessedAsync(int fileId, string fileName)
    {
        try
        {
            _logger.LogInformation("Publishing file processed notification for FileId={FileId}, FileName={FileName}", fileId, fileName);
            var response = await _api.FileProcessedAsync(new FileProcessed(fileId, fileName));
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Notification published successfully for FileId={FileId}", fileId);
            }
            else
            {
                var msg = response.Error?.Content ?? response.Error?.Message ?? "Unknown error";
                _logger.LogWarning("Notification publish failed with status {StatusCode} for FileId={FileId}. Error={Error}", response.StatusCode, fileId, msg);
            }
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "API exception while publishing notification for FileId={FileId}", fileId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected exception while publishing notification for FileId={FileId}", fileId);
        }
    }
}