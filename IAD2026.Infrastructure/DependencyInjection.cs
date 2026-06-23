using Microsoft.Extensions.DependencyInjection;


namespace IAD2026.Infrastructure;

using Microsoft.Extensions.DependencyInjection;


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

        services.AddBackgroundJobs();

        return services;
    }
}