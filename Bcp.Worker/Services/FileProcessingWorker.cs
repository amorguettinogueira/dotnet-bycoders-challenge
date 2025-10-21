using Bcp.Application.Interfaces;
using Bcp.Domain.Enums;
using Bcp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Bcp.Worker.Services;

public class FileProcessingWorker(ILogger<FileProcessingWorker> logger,
                                  ICnabFileProcessor cnabFileProcessor,
                                  AppDbContext db) : BackgroundService
{
    /// <summary>
    /// This is just an example of what we could implement to monitor the instance
    /// before blindly triggering a new file process. Ultimately another library could
    /// be used to monitor multiple resources like disk and memory as well.
    /// </summary>
    /// <returns>True if the instance can handle another file or False if it can't.</returns>
    private static bool CanProcessAnotherFile()
    {
        // CPU usage for current process
        var process = Process.GetCurrentProcess();
        TimeSpan startCpuUsage = process.TotalProcessorTime;
        DateTime startTime = DateTime.UtcNow;

        Thread.Sleep(1000);

        TimeSpan endCpuUsage = process.TotalProcessorTime;
        DateTime endTime = DateTime.UtcNow;

        double cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
        double totalMsPassed = (endTime - startTime).TotalMilliseconds;
        double cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed) * 100;

        return cpuUsageTotal < 80;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var runningTasks = new List<Task>();

        while (!stoppingToken.IsCancellationRequested)
        {
            if (CanProcessAnotherFile())
            {
                var nextFile = await db.FileNotifications.FirstOrDefaultAsync(_ => _.Status == NotificationStatus.Pending, stoppingToken);

                if (nextFile != null)
                {
                    runningTasks.Add(cnabFileProcessor.ProcessAsync(nextFile, stoppingToken));
                    await Task.Delay(1000, stoppingToken);
                }
                else
                {
                    // Housekeeping, kill completed tasks
                    runningTasks.RemoveAll(_ => _.IsCompleted);

                    if (logger.IsEnabled(LogLevel.Information))
                    {
                        logger.LogInformation("Waiting for files. Sleeping for 2 seconds.");
                        await Task.Delay(2000, stoppingToken);
                    }
                }
            }
            else
            {
                if (logger.IsEnabled(LogLevel.Information))
                {
                    logger.LogInformation("Overloaded. Sleeping for 5 seconds.");
                }
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
