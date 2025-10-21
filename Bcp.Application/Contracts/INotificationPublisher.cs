namespace Bcp.Application.Contracts;

public interface INotificationPublisher
{
    Task PublishFileProcessedAsync(int fileId, string fileName);
}
