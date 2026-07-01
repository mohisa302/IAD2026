using System.Text.Json;
using IAD2026.Shared;

namespace IAD2026.Application.Interfaces;

public interface IExternalApiClient
{
    // Strongly typed responses
    Task<TResponse?> GetAsync<TResponse>(string systemKey, string endpoint, CancellationToken ct = default);
    Task<TResponse?> PostAsync<TRequest, TResponse>(string systemKey, string endpoint, TRequest body, CancellationToken ct = default);

    // Dynamic JSON parsing (for unknown/varying structures)
    Task<JsonElement> GetDynamicAsync(string systemKey, string endpoint, CancellationToken ct = default);
    Task<JsonElement> PostDynamicAsync<TRequest>(string systemKey, string endpoint, TRequest body, CancellationToken ct = default);

    // Paginated responses
    Task<PagedResponse<T>> GetPagedAsync<T>(string systemKey, string endpoint, int page = 1, int pageSize = 20, CancellationToken ct = default);
}