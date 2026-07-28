using IAD2026.BackgroundJobs.Jobs;
using IAD2026.Persistence;
using Hangfire;
using Microsoft.Extensions.Options;
using IAD2026.BackgroundJobs.Options;

namespace IAD2026.Api.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UseEnterpriseInitialization(this WebApplication app)
    {
        InitializeInfrastructure(app);
        InitializeHangfire(app);
        return app;
    }

    private static void InitializeInfrastructure(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.EnsureCreated();
    }

    private static void InitializeHangfire(WebApplication app)
    {
        app.UseHangfireDashboard("/hangfire");

        var settings = app.Services.GetRequiredService<IOptions<HangfireSettings>>().Value;

        // Map your config to the job types
        var schedules = new Dictionary<string, string>
    {
        { "SmsNotification", settings.SmsQueueProcessorCron },
        { "DataScrubbing", settings.DataRetentionCleanupCron }
    };

        foreach (var schedule in schedules)
        {
            RecurringJob.AddOrUpdate<OutboxProcessorJob>(
                $"processor-{schedule.Key.ToLower()}",
                job => job.DistributePendingTasksAsync(schedule.Key, CancellationToken.None),
                schedule.Value);

            RecurringJob.AddOrUpdate<SwitchPortSyncJob>(
            "switch-port-sync",
            job => job.Execute(CancellationToken.None),
            settings.SwitchPortSyncCron);
        }
    }
}