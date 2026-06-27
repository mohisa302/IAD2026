using IAD2026.Shared;
using MediatR;

namespace IAD2026.Application.Features.External.Queries;

/// <summary>
/// Query to fetch data from an external system using the integration layer.
/// </summary>
public record FetchExternalDataQuery(
    string SystemKey,
    string Endpoint
) : IRequest<ApiResponse<object?>>;