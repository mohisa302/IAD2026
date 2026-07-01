using System.Text.Json;

namespace IAD2026.Application.Interfaces;

public interface IExternalApiClient
{
    // Strongly typed responses
    Task<TResponse?> GetAsync<TResponse>(string systemKey, string endpoint, CancellationToken ct = default);
    Task<TResponse?> PostAsync<TRequest, TResponse>(string systemKey, string endpoint, TRequest body, CancellationToken ct = default);

    // Dynamic JSON parsing (for unknown/varying structures)
    Task<JsonElement> GetDynamicAsync(string systemKey, string endpoint, CancellationToken ct = default);
    Task<JsonElement> PostDynamicAsync<TRequest>(string systemKey, string endpoint, TRequest body, CancellationToken ct = default);

}