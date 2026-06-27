using IAD2026.Shared;

namespace IAD2026.Application.Interfaces;

public interface IExternalCredentialProvider
{
    Task<ApiCredential> GetCredentialAsync(string systemKey, CancellationToken ct = default);
}