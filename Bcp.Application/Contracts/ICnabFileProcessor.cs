using Bcp.Domain.Models;

namespace Bcp.Application.Interfaces;

public interface ICnabFileProcessor
{
    Task ProcessAsync(FileNotification fileNotification, CancellationToken cancellationToken);
}

