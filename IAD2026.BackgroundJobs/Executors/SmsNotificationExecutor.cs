using IAD2026.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace IAD2026.BackgroundJobs.Executors;

public class SmsNotificationExecutor : ITaskExecutor
{
    private readonly ILogger<SmsNotificationExecutor> _logger;

    public SmsNotificationExecutor(ILogger<SmsNotificationExecutor> logger)
    {
        _logger = logger;
    }

    // This string maps directly to the OutboxTask.TaskType in the database
    public string TaskType => "SmsNotification";

    public async Task ExecuteAsync(string? payload, CancellationToken cancellationToken)
    {
        _logger.LogInformation("🚀 [SMS Executor] Starting SMS dispatch pipeline.");
        Console.WriteLine($"[SMS Executor] Parsing payload: {payload ?? "No payload"}");
        Console.WriteLine("[SMS Executor] Dispatching SMS to telecom gateway...");

        // Simulate network latency for the API call
        await Task.Delay(500, cancellationToken);

        _logger.LogInformation("✅ [SMS Executor] SMS dispatched successfully.");
        Console.WriteLine("[SMS Executor] SMS dispatched successfully.");
    }
}