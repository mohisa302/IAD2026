using Microsoft.Extensions.DependencyInjection;
namespace IAD2026.Caching;

public static class DependencyInjection
{
    public static IServiceCollection AddCaching(
        this IServiceCollection services)
    {
        return services;
    }
}