using IAD2026.Application.Interfaces;
using IAD2026.Application.Options;
using IAD2026.Shared.Models;
using Microsoft.Extensions.Options;

namespace IAD2026.Integrations.Credentials;

public class ConfigurationCredentialProvider : IExternalCredentialProvider
{
    private readonly ExternalApiOptions _options;

    public ConfigurationCredentialProvider(IOptions<ExternalApiOptions> options)
    {
        _options = options.Value;
    }

    public Task<ApiCredential> GetCredentialAsync(string systemKey, CancellationToken ct = default)
    {
        if (!_options.Systems.TryGetValue(systemKey, out var system))
        {
            throw new InvalidOperationException($"External system '{systemKey}' is not configured in ExternalSystems section.");
        }

        // Validate AuthType
        if (!Enum.IsDefined(typeof(AuthType), system.AuthType))
        {
            throw new InvalidOperationException(
                $"Invalid AuthType '{system.AuthType}' configured for system '{systemKey}'. " +
                "Allowed values: ApiKey, Bearer, Basic.");
        }

        var credential = new ApiCredential(
            SystemKey: systemKey,
            BaseUrl: system.BaseUrl,
            AuthType: system.AuthType,
            ApiKey: system.ApiKey,
            ApiKeyHeaderName: system.ApiKeyHeaderName,   // ← Comes from config
            Username: system.Username,
            Password: system.Password
        );

        return Task.FromResult(credential);
    }
}