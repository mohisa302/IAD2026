using IAD2026.Application.Attributes;
using IAD2026.Application.Mappings;
using System.Reflection;
using System.Text.Json;

namespace IAD2026.Application.Services;

public class PaginatedFetcher : IPaginatedFetcher
{
    public async Task<List<T>> FetchAllAsync<T>(
        Func<int, int, CancellationToken, Task<JsonElement>> getPageAsync,
        string? itemsPropertyName = null,
        string? totalPropertyName = null,
        int defaultPageSize = 50,
        CancellationToken ct = default)
        where T : new()
    {
        if (itemsPropertyName == null || totalPropertyName == null)
        {
            var attr = typeof(T).GetCustomAttribute<PaginatedResponseAttribute>();
            itemsPropertyName ??= attr?.ItemsProperty ?? "issues";
            totalPropertyName ??= attr?.TotalProperty ?? "total";
        }

        var allItems = new List<T>();
        int currentPage = 1;
        int totalCount = 0;

        while (true)
        {
            var pageJson = await getPageAsync(currentPage, defaultPageSize, ct);

            if (currentPage == 1 &&
                pageJson.TryGetProperty(totalPropertyName, out var totalProp))
            {
                totalCount = totalProp.GetInt32();
            }

            if (pageJson.TryGetProperty(itemsPropertyName, out var itemsArray) &&
                itemsArray.ValueKind == JsonValueKind.Array)
            {
                foreach (var itemJson in itemsArray.EnumerateArray())
                {
                    allItems.Add(JsonElementMapper.Map<T>(itemJson));
                }

                if (allItems.Count >= totalCount || itemsArray.GetArrayLength() == 0)
                    break;
            }
            else
            {
                break;
            }

            currentPage++;
        }

        return allItems;
    }
}