using Bcp.Application.Contracts;
using Bcp.Domain.Models;
using Bcp.Infrastructure.Persistence;
using Microsoft.Extensions.Logging;

namespace Bcp.Infrastructure.Services;

public class FileTransactionResolver(AppDbContext db,
                                     ILogger<FileTransactionResolver> logger) : IFileTransactionResolver
{
    private readonly string _uploadPath = Environment.GetEnvironmentVariable("UPLOADS_DIR") ?? "/app/uploads";

    public async Task SaveFileAsync(FileNotification fileNotification,
                                  List<Transaction> transactions,
                                  List<FileError> error,
                                  CancellationToken cancellationToken)
    {
        using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            logger.LogInformation($"Starting to save file data for: {fileNotification.FileName}");

            var existingFile = db.Files.FirstOrDefault(_ => _.FileName == Path.GetFileName(fileNotification.FileName));
            if (existingFile != null)
            {
                _ = db.Remove(existingFile);
            }

            // Save file metadata
            var savedFile = new Domain.Models.File
            {
                FileName = Path.GetFileName(fileNotification.FileName), // Store just the filename in the database
                Transactions = transactions,
                Error = error,
            };

            _ = db.Files.Add(savedFile);

            fileNotification.Status = Domain.Enums.NotificationStatus.Completed;
            _ = db.FileNotifications.Update(fileNotification);

            await db.SaveChangesAsync(cancellationToken);

            // Commit DB transaction
            await transaction.CommitAsync(cancellationToken);

            try
            {
                // Resolve full path for deletion
                var fullPath = Path.IsPathRooted(fileNotification.FileName)
                    ? fileNotification.FileName
                    : Path.Combine(_uploadPath, fileNotification.FileName);

                // Delete file from disk using the full path
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                    logger.LogInformation($"Successfully deleted file: {fullPath}");
                }
                else
                {
                    logger.LogWarning($"File not found for deletion: {fullPath}");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed to delete file {fileNotification.FileName}, but database transaction was successful.");
                // Don't throw - we don't want to roll back the DB transaction if file deletion fails
            }

            // Note: publishing notifications to SignalR is handled by the API. The worker/infrastructure
            // updates the FileNotification status to Completed and persists data. The API monitors
            // the database or exposes an endpoint to receive notifications from the worker and then
            // broadcasts via SignalR. Keep infrastructure free of SignalR concerns.
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);

            if (System.IO.File.Exists(fileNotification.FileName))
            {
                fileNotification.Status = Domain.Enums.NotificationStatus.Pending;
                _ = db.FileNotifications.Update(fileNotification);
                _ = await db.SaveChangesAsync(cancellationToken);
            }

            logger.LogError(ex, $"Failed to process file {fileNotification.FileName}. Rolled back DB transaction.");
        }
    }
}
