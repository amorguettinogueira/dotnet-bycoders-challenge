using Bcp.Application.Contracts;
using Bcp.Application.Interfaces;
using Bcp.Domain.Models;
using Bcp.Infrastructure.Contracts;
using Bcp.Infrastructure.Models;
using Bcp.Infrastructure.Persistence;
using FileHelpers;
using Microsoft.Extensions.Logging;

namespace Bcp.Infrastructure.Services;

public class CnabFileProcessor(AppDbContext db,
                               ITransactionParser transactionParser,
                               IFileTransactionResolver fileTransactionResolver,
                               ILogger<CnabFileProcessor> logger,
                               INotificationPublisher? publisher = null) : ICnabFileProcessor
{
    private readonly static int BatchSize = 50; // Number of records per batch

    private readonly string _uploadPath = Environment.GetEnvironmentVariable("UPLOADS_DIR") ?? "/app/uploads";

    private async Task<bool> UpdateNotificationStatusAsync(FileNotification fileNotification, CancellationToken cancellationToken)
    {
        try
        {
            fileNotification.Status = Domain.Enums.NotificationStatus.Processing;
            _ = db.FileNotifications.Update(fileNotification);
            _ = await db.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Failed to update notification status for file {fileNotification.FileName}.");
        }
        return false;
    }

    public async Task ProcessAsync(FileNotification fileNotification, CancellationToken cancellationToken)
    {
        if (!await UpdateNotificationStatusAsync(fileNotification, cancellationToken))
        {
            return;
        }

        var transactions = new List<Transaction>();
        var error = new List<FileError>();

        try
        {
            // Resolve full path within the worker container
            var fullPath = Path.IsPathRooted(fileNotification.FileName)
                ? fileNotification.FileName
                : Path.Combine(_uploadPath, fileNotification.FileName);

            var engine = new FileHelperAsyncEngine<TransactionRecord>();
            try
            {
                logger.LogInformation($"Starting to process file: {fullPath}");

                using var position = engine.BeginReadFile(fullPath);
                engine.ErrorManager.ErrorMode = ErrorMode.SaveAndContinue;

                while (!cancellationToken.IsCancellationRequested)
                {
                    var batch = engine.ReadNexts(BatchSize);

                    if ((batch?.Length ?? 0) == 0)
                    {
                        break;
                    }

                    var batchTransactions = await transactionParser.ParseBatchAsync(batch!, cancellationToken).ConfigureAwait(false);
                    transactions.AddRange(batchTransactions);
                }
            }
            finally
            {
                if (engine.ErrorManager.HasErrors)
                {
                    error.AddRange(engine.ErrorManager.Errors.Select(_ => new FileError()
                    {
                        Error = _.ExceptionInfo.Message,
                        LineNumber = _.LineNumber,
                    }));
                }
                engine.Close();
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Failed to process file {fileNotification.FileName}.");
            error.Add(new FileError()
            {
                Error = $"Failed to process file: {ex.Message}",
            });
        }

        await fileTransactionResolver.SaveFileAsync(fileNotification, transactions, error, cancellationToken);

        // After persisting, optionally notify API via INotificationPublisher if available (worker will register an HTTP implementation)
        try
        {
            if (publisher != null)
            {
                await publisher.PublishFileProcessedAsync(fileNotification.FileNotificationId, fileNotification.FileName);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to notify API about processed file.");
        }
    }
}
