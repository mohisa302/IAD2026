using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using IAD2026.Application.Exceptions;
using IAD2026.Application.Interfaces;
using IAD2026.Shared;

namespace IAD2026.Integrations.Clients;

public class ResilientExternalApiClient : IExternalApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IExternalCredentialProvider _credentialProvider;

    public ResilientExternalApiClient(
        IHttpClientFactory httpClientFactory,
        IExternalCredentialProvider credentialProvider)
    {
        _httpClientFactory = httpClientFactory;
        _credentialProvider = credentialProvider;
    }

    // ==================== Strongly Typed ====================
    public async Task<TResponse?> GetAsync<TResponse>(string systemKey, string endpoint, CancellationToken ct = default)
    {
        var credential = await _credentialProvider.GetCredentialAsync(systemKey, ct);
        var client = CreateClient(credential);

        var response = await client.GetAsync(endpoint, ct);
        await EnsureSuccessStatusAsync(response);

        var content = await response.Content.ReadAsStringAsync(ct);
        return JsonSerializer.Deserialize<TResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string systemKey, string endpoint, TRequest body, CancellationToken ct = default)
    {
        var credential = await _credentialProvider.GetCredentialAsync(systemKey, ct);
        var client = CreateClient(credential);

        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(endpoint, content, ct);
        await EnsureSuccessStatusAsync(response);

        var responseContent = await response.Content.ReadAsStringAsync(ct);
        return JsonSerializer.Deserialize<TResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

    // ==================== Dynamic JSON ====================
    public async Task<JsonElement> GetDynamicAsync(string systemKey, string endpoint, CancellationToken ct = default)
    {
        var credential = await _credentialProvider.GetCredentialAsync(systemKey, ct);
        var client = CreateClient(credential);

        var response = await client.GetAsync(endpoint, ct);
        await EnsureSuccessStatusAsync(response);

        var content = await response.Content.ReadAsStringAsync(ct);
        using var doc = JsonDocument.Parse(content);
        return doc.RootElement.Clone();
    }

    public async Task<JsonElement> PostDynamicAsync<TRequest>(string systemKey, string endpoint, TRequest body, CancellationToken ct = default)
    {
        var credential = await _credentialProvider.GetCredentialAsync(systemKey, ct);
        var client = CreateClient(credential);

        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(endpoint, content, ct);
        await EnsureSuccessStatusAsync(response);

        var responseContent = await response.Content.ReadAsStringAsync(ct);
        using var doc = JsonDocument.Parse(responseContent);
        return doc.RootElement.Clone();
    }

    // ==================== Paginated ====================
    public async Task<PagedResponse<T>> GetPagedAsync<T>(string systemKey, string endpoint, int page = 1, int pageSize = 20, CancellationToken ct = default)
    {
        var credential = await _credentialProvider.GetCredentialAsync(systemKey, ct);
        var client = CreateClient(credential);

        var pagedEndpoint = $"{endpoint}?page={page}&pageSize={pageSize}";

        var response = await client.GetAsync(pagedEndpoint, ct);
        await EnsureSuccessStatusAsync(response);

        var content = await response.Content.ReadAsStringAsync(ct);
        var pagedResult = JsonSerializer.Deserialize<PagedResponse<T>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return pagedResult ?? PagedResponse<T>.Error("PARSING_ERROR", "Failed to parse paginated response", 500);
    }

    // ==================== Private Helpers ====================
    private HttpClient CreateClient(ApiCredential credential)
    {
        var client = _httpClientFactory.CreateClient("ExternalApi");
        client.BaseAddress = new Uri(credential.BaseUrl.TrimEnd('/') + "/");
        AddAuthenticationHeaders(client, credential);
        return client;
    }

    private static void AddAuthenticationHeaders(HttpClient client, ApiCredential credential)
    {
        client.DefaultRequestHeaders.Clear();

        switch (credential.AuthType.ToLowerInvariant())
        {
            case "apikey":
                if (!string.IsNullOrEmpty(credential.ApiKey))
                    client.DefaultRequestHeaders.Add("X-Api-Key", credential.ApiKey);
                break;

            case "bearer":
                if (!string.IsNullOrEmpty(credential.Token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", credential.Token);
                break;

            case "basic":
                if (!string.IsNullOrEmpty(credential.Username) && !string.IsNullOrEmpty(credential.Password))
                {
                    var basicAuth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{credential.Username}:{credential.Password}"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicAuth);
                }
                break;
        }
    }

    private static async Task EnsureSuccessStatusAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
            return;

        var content = await response.Content.ReadAsStringAsync();
        var errorMessage = string.IsNullOrWhiteSpace(content)
            ? $"External API returned {(int)response.StatusCode} {response.ReasonPhrase}"
            : content;

        var errorCode = GetErrorCode(response.StatusCode);

        throw new ExternalApiException(errorMessage, response.StatusCode, errorCode);
    }

    private static string GetErrorCode(HttpStatusCode statusCode) => statusCode switch
    {
        HttpStatusCode.BadRequest => ErrorCodes.ExternalBadRequest,
        HttpStatusCode.Unauthorized => ErrorCodes.ExternalUnauthorized,
        HttpStatusCode.Forbidden => ErrorCodes.ExternalForbidden,
        HttpStatusCode.NotFound => ErrorCodes.ExternalNotFound,
        HttpStatusCode.TooManyRequests => ErrorCodes.ExternalRateLimited,
        HttpStatusCode.InternalServerError => ErrorCodes.ExternalServerError,
        HttpStatusCode.BadGateway => ErrorCodes.ExternalBadGateway,
        HttpStatusCode.ServiceUnavailable => ErrorCodes.ExternalServiceUnavailable,
        HttpStatusCode.GatewayTimeout => ErrorCodes.ExternalGatewayTimeout,
        _ => ErrorCodes.ExternalApiError
    };
}