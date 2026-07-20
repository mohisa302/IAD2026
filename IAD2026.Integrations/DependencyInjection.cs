using IAD2026.Application.Interfaces;
using IAD2026.Integrations.Clients;
using IAD2026.Integrations.Credentials;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Polly;
using System.Net;

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
        services.AddHttpClient("ExternalApi", client =>
        {
            client.Timeout = TimeSpan.FromMinutes(10);
        })
        .AddResilienceHandler("external-api-pipeline", builder =>
        {
            builder.AddRetry(new HttpRetryStrategyOptions
            {
                MaxRetryAttempts = 5,
                Delay = TimeSpan.FromSeconds(2),
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    .Handle<HttpRequestException>()
                    .HandleResult(r =>
                        r.StatusCode == HttpStatusCode.TooManyRequests ||
                        r.StatusCode == HttpStatusCode.ServiceUnavailable ||
                        r.StatusCode == HttpStatusCode.BadGateway)
            });

            builder.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
            {
                SamplingDuration = TimeSpan.FromSeconds(30),
                FailureRatio = 0.5,
                MinimumThroughput = 10
            });

            builder.AddTimeout(TimeSpan.FromMinutes(10));
        });
        // 3. Register the API Client
        services.AddScoped<IExternalApiClient, ResilientExternalApiClient>();

        return services;
    }
}