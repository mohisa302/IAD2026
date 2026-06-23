using Microsoft.Extensions.DependencyInjection;

namespace IAD2026.BackgroundJobs;
public static class DependencyInjection
{
    public static IServiceCollection AddBackgroundJobs(
        this IServiceCollection services)
    {
        return services;
    }
}