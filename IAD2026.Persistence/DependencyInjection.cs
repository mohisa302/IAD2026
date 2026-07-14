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
        services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(
            configuration.GetConnectionString("DefaultConnection")));


        // 1. Register generic repository explicitly
        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

        // 2. Auto-Register all custom repositories using Reflection
        var persistenceAssembly = typeof(DependencyInjection).Assembly;

        var repositoryTypes = persistenceAssembly.GetTypes()
            .Where(t => t.IsClass
                     && !t.IsAbstract
                     && t.Name.EndsWith("Repository")
                     && t.Name != "EfRepository`1"); // Exclude generic base

        foreach (var type in repositoryTypes)
        {
            // Find the matching interface (e.g., OutboxRepository -> IOutboxRepository)
            var interfaceType = type.GetInterfaces()
                .FirstOrDefault(i => i.Name == $"I{type.Name}");

            if (interfaceType != null)
            {
                services.AddScoped(interfaceType, type);
            }
        }

        return services;
    }
}