namespace IAD2026.Application.Interfaces;

public interface IExternalApiClient
{
    Task<TResponse?> GetAsync<TResponse>(
        string systemKey,
        string endpoint,
        CancellationToken ct = default);

    Task<TResponse?> PostAsync<TRequest, TResponse>(
        string systemKey,
        string endpoint,
        TRequest body,
        CancellationToken ct = default);
}