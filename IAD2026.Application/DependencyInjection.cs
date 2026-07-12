using IAD2026.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace IAD2026.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        });
        services.AddScoped<IPaginatedFetcher, PaginatedFetcher>();

        return services;
    }
}