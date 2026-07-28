using Hangfire;
using Hangfire.InMemory; // Or Hangfire.SqlServer for production
using Hangfire.SqlServer;
using IAD2026.Application.Interfaces;
using IAD2026.BackgroundJobs.Executors;
using IAD2026.BackgroundJobs.Jobs;
using IAD2026.BackgroundJobs.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IAD2026.BackgroundJobs;

public static class DependencyInjection
{
    public static IServiceCollection AddBackgroundJobs(
    this IServiceCollection services,
    IConfiguration configuration)
{
    services.Configure<HangfireSettings>(
        configuration.GetSection("HangfireSettings"));

    services.AddHangfire(config =>
    {
        config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            config.UseInMemoryStorage();
        }
        else
        {
            config.UseSqlServerStorage(
                connectionString,
                new SqlServerStorageOptions
                {
                    PrepareSchemaIfNecessary = true,
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.FromSeconds(15),
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                });
        }
    });

    services.AddScoped<ITaskExecutor, SmsNotificationExecutor>();
    services.AddScoped<ITaskExecutor, DatabaseRetentionExecutor>();

    services.AddScoped<OutboxProcessorJob>();
    services.AddScoped<SwitchPortSyncJob>();

    services.AddHangfireServer(options =>
    {
        options.WorkerCount = Environment.ProcessorCount * 2;
        options.Queues = new[] { "default" };
    });

    return services;
}
}