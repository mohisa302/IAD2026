using IAD2026.Application.Interfaces;
using IAD2026.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace IAD2026.Application.Features.External.Queries;

public class FetchExternalDataHandler : IRequestHandler<FetchExternalDataQuery, ApiResponse<object?>>
{
    private readonly IExternalApiClient _apiClient;
    private readonly ILogger<FetchExternalDataHandler> _logger;

    public FetchExternalDataHandler(
        IExternalApiClient apiClient,
        ILogger<FetchExternalDataHandler> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    public async Task<ApiResponse<object?>> Handle(FetchExternalDataQuery request, CancellationToken ct)
    {
        try
        {
            _logger.LogInformation("Fetching data from external system. SystemKey: {SystemKey}, Endpoint: {Endpoint}",
                request.SystemKey, request.Endpoint);

            var result = await _apiClient.GetAsync<object>(request.SystemKey, request.Endpoint, ct);

            _logger.LogInformation("Successfully fetched data from external system {SystemKey}", request.SystemKey);

            return ApiResponse<object?>.Success(result);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            _logger.LogWarning("Unauthorized (401) from external system {SystemKey}", request.SystemKey);
            return ApiResponse<object?>.ErrorResponse("EXTERNAL_UNAUTHORIZED", "External system returned 401 Unauthorized", 401);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch data from external system {SystemKey}", request.SystemKey);
            return ApiResponse<object?>.ErrorResponse("EXTERNAL_API_ERROR", "Failed to call external system", 502);
        }
    }
}