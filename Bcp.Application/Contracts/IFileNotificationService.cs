namespace Bcp.Application.Contracts;

public interface IFileNotificationService
{
    Task NotifyAsync(string fileName);
}
