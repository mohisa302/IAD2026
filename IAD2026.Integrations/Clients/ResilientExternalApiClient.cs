using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
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

    public async Task<TResponse?> GetAsync<TResponse>(string systemKey, string endpoint, CancellationToken ct = default)
    {
        var credential = await _credentialProvider.GetCredentialAsync(systemKey, ct);
        var client = CreateClient(credential);

        var response = await client.GetAsync(endpoint, ct);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(ct);
        return JsonSerializer.Deserialize<TResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(
        string systemKey,
        string endpoint,
        TRequest body,
        CancellationToken ct = default)
    {
        var credential = await _credentialProvider.GetCredentialAsync(systemKey, ct);
        var client = CreateClient(credential);

        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(endpoint, content, ct);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync(ct);
        return JsonSerializer.Deserialize<TResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

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
}