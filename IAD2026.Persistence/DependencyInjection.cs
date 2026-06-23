using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IAD2026.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services;
    }
}