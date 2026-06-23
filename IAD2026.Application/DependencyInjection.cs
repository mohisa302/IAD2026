using Microsoft.Extensions.DependencyInjection;

namespace IAD2026.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        return services;
    }
}