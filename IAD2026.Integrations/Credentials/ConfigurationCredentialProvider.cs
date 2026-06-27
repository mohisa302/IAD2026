using IAD2026.Application.Interfaces;
using IAD2026.Shared;
using Microsoft.Extensions.Configuration;

namespace IAD2026.Integrations.Credentials;

public class ConfigurationCredentialProvider : IExternalCredentialProvider
{
    private readonly IConfiguration _config;


    public ConfigurationCredentialProvider(IConfiguration config)
    {
        _config = config;
    }

    public Task<ApiCredential> GetCredentialAsync(string systemKey, CancellationToken ct = default)
    {
        var section = _config.GetSection($"ExternalSystems:{systemKey}");

        var credential = new ApiCredential(
            SystemKey: systemKey,
            BaseUrl: section["BaseUrl"] ?? "",
            AuthType: section["AuthType"] ?? "ApiKey",
            ApiKey: section["ApiKey"],
            Username: section["Username"],
            Password: section["Password"]
        );

        return Task.FromResult(credential);
    }
}