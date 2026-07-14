namespace IAD2026.BackgroundJobss.Options;

public class HangfireSettings
{
    public string DataRetentionCleanupCron { get; set; } = "0 2 * * *"; // Default fallback
    public string SmsQueueProcessorCron { get; set; } = "*/1 * * * *";
}