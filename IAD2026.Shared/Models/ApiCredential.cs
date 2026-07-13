namespace IAD2026.Shared.Models;

public record ApiCredential(
    string SystemKey,
    string BaseUrl,
    AuthType AuthType,
    string? ApiKey = null,
    string ApiKeyHeaderName = "X-Api-Key",     
    string? Username = null,
    string? Password = null,
    string? Token = null
);