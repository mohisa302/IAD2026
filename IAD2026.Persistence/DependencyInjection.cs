using IAD2026.Application.Interfaces;
using IAD2026.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IAD2026.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // For now we use In-Memory for template. Later we will add real DB.
        services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("IAD2026InMemoryDb"));
        // Register generic repository
        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        services.AddScoped<IOutboxRepository, OutboxRepository>();
        return services;
    }
}