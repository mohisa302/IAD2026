using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using IAD2026.Application.Exceptions;
using IAD2026.Application.Interfaces;
using IAD2026.Shared;
using IAD2026.Shared.Models;
using Microsoft.Extensions.Logging;   // ← Use this

namespace IAD2026.Integrations.Clients;

public class ResilientExternalApiClient : IExternalApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IExternalCredentialProvider _credentialProvider;
    private readonly ILogger<ResilientExternalApiClient> _logger;   // ← Injected logger

    public ResilientExternalApiClient(
        IHttpClientFactory httpClientFactory,
        IExternalCredentialProvider credentialProvider,
        ILogger<ResilientExternalApiClient> logger)
    {
        _httpClientFactory = httpClientFactory;
        _credentialProvider = credentialProvider;
        _logger = logger;
    }

    // ==================== Strongly Typed ====================
    public async Task<TResponse?> GetAsync<TResponse>(string systemKey, string endpoint, CancellationToken ct = default)
    {
        var credential = await _credentialProvider.GetCredentialAsync(systemKey, ct);
        var client = CreateClient(credential);

        var response = await client.GetAsync(endpoint, ct);
        await EnsureSuccessStatusAsync(response, systemKey, endpoint, null);

        var content = await response.Content.ReadAsStringAsync(ct);
        _logger.LogInformation(
    "Response string length: {Length}",
    content.Length);

_logger.LogInformation(
    "Response starts with: {Start}",
    content[..Math.Min(200, content.Length)]);

_logger.LogInformation(
    "Response ends with: {End}",
    content[^Math.Min(200, content.Length)..]);

        return JsonSerializer.Deserialize<TResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string systemKey, string endpoint, TRequest body, CancellationToken ct = default)
    {
        var credential = await _credentialProvider.GetCredentialAsync(systemKey, ct);
        var client = CreateClient(credential);

        var requestBodyJson = JsonSerializer.Serialize(body);
        var content = new StringContent(requestBodyJson, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(endpoint, content, ct);
        await EnsureSuccessStatusAsync(response, systemKey, endpoint, requestBodyJson);

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
        await EnsureSuccessStatusAsync(response, systemKey, endpoint, null);

        var content = await response.Content.ReadAsStringAsync(ct);
        using var doc = JsonDocument.Parse(content);
        return doc.RootElement.Clone();
    }

    public async Task<JsonElement> PostDynamicAsync<TRequest>(string systemKey, string endpoint, TRequest body, CancellationToken ct = default)
    {
        var credential = await _credentialProvider.GetCredentialAsync(systemKey, ct);
        var client = CreateClient(credential);

        var requestBodyJson = JsonSerializer.Serialize(body);
        var content = new StringContent(requestBodyJson, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(endpoint, content, ct);
        await EnsureSuccessStatusAsync(response, systemKey, endpoint, requestBodyJson);

        var responseContent = await response.Content.ReadAsStringAsync(ct);
        using var doc = JsonDocument.Parse(responseContent);
        return doc.RootElement.Clone();
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

        switch (credential.AuthType)
        {
            case AuthType.ApiKey:
                if (!string.IsNullOrEmpty(credential.ApiKey))
                    client.DefaultRequestHeaders.Add("X-Api-Key", credential.ApiKey);
                break;

            case AuthType.Bearer:
                if (!string.IsNullOrEmpty(credential.Token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", credential.Token);
                break;

            case AuthType.Basic:
                if (!string.IsNullOrEmpty(credential.Username) && !string.IsNullOrEmpty(credential.Password))
                {
                    var basicAuth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{credential.Username}:{credential.Password}"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicAuth);
                }
                break;
        }
    }

    // ==================== ONLY LOG ON EXCEPTION ====================
    private async Task EnsureSuccessStatusAsync(HttpResponseMessage response, string systemKey, string endpoint, string? requestBody)
    {
        if (response.IsSuccessStatusCode)
            return;

        var responseBody = await response.Content.ReadAsStringAsync();

        // Log only failures with full details
        _logger.LogError("External API Error | System: {SystemKey} | Endpoint: {Endpoint} | Status: {StatusCode} | RequestBody: {RequestBody} | ResponseBody: {ResponseBody}",
            systemKey, endpoint, (int)response.StatusCode, requestBody ?? "N/A", responseBody);

        var errorMessage = string.IsNullOrWhiteSpace(responseBody)
            ? $"External API returned {(int)response.StatusCode} {response.ReasonPhrase}"
            : responseBody;

        var errorCode = GetErrorCode(response.StatusCode);

        throw new ExternalApiException(errorMessage, response.StatusCode, errorCode)
        {
            RawRequest = requestBody,
            RawResponse = responseBody
        };
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