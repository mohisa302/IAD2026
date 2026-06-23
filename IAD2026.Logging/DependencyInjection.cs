
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IAD2026.Logging;
public static class DependencyInjection
{
    public static IServiceCollection AddLoggingModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services;
    }

}
