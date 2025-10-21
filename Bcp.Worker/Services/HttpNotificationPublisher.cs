using Bcp.Application.Contracts;
using Bcp.Application.DTOs;
using Bcp.Worker.Contracts;
using Refit;

namespace Bcp.Worker.Services;

public class HttpNotificationPublisher(INotificationsApi api, ILogger<HttpNotificationPublisher> logger) : INotificationPublisher
{
    public async Task PublishFileProcessedAsync(int fileId, string fileName)
    {
        try
        {
            logger.LogInformation("Publishing file processed notification for FileId={FileId}, FileName={FileName}", fileId, fileName);
            var response = await api.FileProcessedAsync(new FileProcessed(fileId, fileName));
            if (response.IsSuccessStatusCode)
            {
                logger.LogInformation("Notification published successfully for FileId={FileId}", fileId);
            }
            else
            {
                var msg = response.Error?.Content ?? response.Error?.Message ?? "Unknown error";
                logger.LogWarning("Notification publish failed with status {StatusCode} for FileId={FileId}. Error={Error}", response.StatusCode, fileId, msg);
            }
        }
        catch (ApiException ex)
        {
            logger.LogError(ex, "API exception while publishing notification for FileId={FileId}", fileId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected exception while publishing notification for FileId={FileId}", fileId);
        }
    }
}