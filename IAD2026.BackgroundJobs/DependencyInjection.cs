using Hangfire;
using Hangfire.InMemory; // Or Hangfire.SqlServer for production
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
        this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<HangfireSettings>(configuration.GetSection("HangfireSettings"));

        // 1. Add Hangfire Core & Storage
        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseInMemoryStorage()); // Replace with .UseSqlServerStorage() in production

        // 2. Register your job classes and strategy executors into the DI container
        services.AddScoped<ITaskExecutor, SmsNotificationExecutor>();
        services.AddScoped<ITaskExecutor, DatabaseRetentionExecutor>();
        services.AddScoped<OutboxProcessorJob>();
        services.AddScoped<SwitchPortSyncJob>();
        // 3. Register the Hangfire Hosted Server (this node will actively process jobs)
        services.AddHangfireServer(options =>
        {
            options.WorkerCount = Environment.ProcessorCount * 2;
            options.Queues = new[] { "default" }; // Ensure "default" is here for basic routing
        });

        return services;
    }
}