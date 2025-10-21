using Bcp.Domain.Models;

namespace Bcp.Application.Contracts;

public interface IFileTransactionResolver
{
    Task SaveFileAsync(FileNotification fileNotification, List<Transaction> transactions, List<FileError> error, CancellationToken cancellationToken);
}