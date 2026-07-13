using IAD2026.BackgroundJobs;
using IAD2026.Caching;
using IAD2026.Integrations;
using IAD2026.Logging;
using IAD2026.Persistence;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IAD2026.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddPersistence(configuration);

        services.AddCaching();

        services.AddIntegrations(configuration);

        services.AddLoggingModule(configuration);

        services.AddBackgroundJobs(configuration);

        return services;
    }
}