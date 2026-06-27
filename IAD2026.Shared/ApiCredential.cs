namespace IAD2026.Shared;

public record ApiCredential(
    string SystemKey,
    string BaseUrl,
    string AuthType,           // "ApiKey", "Bearer", "Basic", "OAuth2"
    string? ApiKey = null,
    string? Username = null,
    string? Password = null,
    string? Token = null
);