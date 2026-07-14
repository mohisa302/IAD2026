using IAD2026.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace IAD2026.BackgroundJobs.Executors;

public class DatabaseRetentionExecutor : ITaskExecutor
{
    private readonly ILogger<DatabaseRetentionExecutor> _logger;

    public DatabaseRetentionExecutor(ILogger<DatabaseRetentionExecutor> logger)
    {
        _logger = logger;
    }

    // This string maps directly to the OutboxTask.TaskType in the database
    public string TaskType => "DataScrubbing";

    public async Task ExecuteAsync(string? payload, CancellationToken cancellationToken)
    {
        _logger.LogInformation("🧹 [Data Retention Executor] Starting stale data cleanup.");
        Console.WriteLine("[Data Retention Executor] Scanning for expired CDRs and stale cache entries...");

        // Simulate heavy database I/O execution time
        await Task.Delay(1000, cancellationToken);

        _logger.LogInformation("✅ [Data Retention Executor] Cleanup completed successfully.");
        Console.WriteLine("[Data Retention Executor] Cleanup completed successfully.");
    }
}