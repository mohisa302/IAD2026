using IAD2026.Application.Interfaces;
using IAD2026.BackgroundJobs.Jobs;
using IAD2026.BackgroundJobss.Options;
using IAD2026.Domain.Entities;
using IAD2026.Persistence;
using Hangfire;
using Microsoft.Extensions.Options;

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

        if (!db.TaskQueue.Any())
        {
            db.TaskQueue.AddRange(
                new OutboxTask { TaskType = "SmsNotification", ReferenceId = Guid.NewGuid().ToString(), Status = OutboxTaskStatus.Pending, Payload = "Test SMS Payload" },
                new OutboxTask { TaskType = "DataScrubbing", ReferenceId = Guid.NewGuid().ToString(), Status = OutboxTaskStatus.Pending, Payload = "Test Scrubbing Payload" }
            );
            db.SaveChanges();
        }
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
        }
    }
}