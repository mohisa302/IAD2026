using IAD2026.Application.Interfaces;
using IAD2026.Integrations.Clients;
using IAD2026.Integrations.Credentials;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Polly;

namespace IAD2026.Integrations;

public static class DependencyInjection
{
    public static IServiceCollection AddIntegrations(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // 1. Credential Provider
        services.AddScoped<IExternalCredentialProvider, ConfigurationCredentialProvider>();

        // 2. Resilient Http Client (Polly is already here via AddResilienceHandler)
        services.AddHttpClient("ExternalApi")
            .AddResilienceHandler("external-api-pipeline", builder =>
            {
                builder.AddRetry(new HttpRetryStrategyOptions
                {
                    MaxRetryAttempts = 3,
                    Delay = TimeSpan.FromSeconds(1),
                    BackoffType = DelayBackoffType.Exponential
                });
                builder.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
                {
                    SamplingDuration = TimeSpan.FromSeconds(30),
                    FailureRatio = 0.5,
                    MinimumThroughput = 10
                });
                builder.AddTimeout(TimeSpan.FromSeconds(30));
            });

        // 3. Register the API Client
        services.AddScoped<IExternalApiClient, ResilientExternalApiClient>();

        return services;
    }
}