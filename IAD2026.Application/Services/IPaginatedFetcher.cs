using System.Text.Json;

namespace IAD2026.Application.Services;

public interface IPaginatedFetcher
{
    Task<List<T>> FetchAllAsync<T>(
        Func<int, int, CancellationToken, Task<JsonElement>> getPageAsync,
        string? itemsPropertyName = null,
        string? totalPropertyName = null,
        int defaultPageSize = 50,
        CancellationToken ct = default) where T : new();

    Task FetchAllRawAsync(
    Func<int, int, CancellationToken, Task<JsonElement>> getPageAsync,
    Func<JsonElement, int, CancellationToken, Task> savePageAsync,
    string totalPropertyName = "total_count",
    int defaultPageSize = 100,
    CancellationToken ct = default);
}