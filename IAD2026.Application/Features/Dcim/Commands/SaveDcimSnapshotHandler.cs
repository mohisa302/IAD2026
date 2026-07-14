using IAD2026.Application.Interfaces;
using IAD2026.Application.Services;
using IAD2026.Domain.Entities;
using IAD2026.Domain.Enums;
using IAD2026.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace IAD2026.Application.Features.Dcim.Commands;

public class SaveDcimSnapshotHandler
    : IRequestHandler<SaveDcimSnapshotCommand, ApiResponse<object?>>
{
    private readonly IExternalApiClient _apiClient;
    private readonly IPaginatedFetcher _fetcher;
    private readonly IDcimRepository _repository;
    private readonly ILogger<SaveDcimSnapshotHandler> _logger;


    public SaveDcimSnapshotHandler(
        IExternalApiClient apiClient,
        IPaginatedFetcher fetcher,
        IDcimRepository repository,
        ILogger<SaveDcimSnapshotHandler> logger)
    {
        _apiClient = apiClient;
        _fetcher = fetcher;
        _repository = repository;
        _logger = logger;
    }


    public async Task<ApiResponse<object?>> Handle(
        SaveDcimSnapshotCommand request,
        CancellationToken ct)
    {
        try
        {
            var endpoint = GetEndpoint(request.DcimType);

            _logger.LogInformation(
                "Fetching DCIM snapshot {Type}",
                request.DcimType);


            int pagesSaved = 0;


            await _fetcher.FetchAllRawAsync(
                async (page, pageSize, token) =>
                {
                    var offset =
                        (page - 1) * pageSize;


                    return await _apiClient.GetDynamicAsync(
                        "DCIM",
                        $"{endpoint}?limit={pageSize}&offset={offset}",
                        token);
                },


                async (response, page, token) =>
                {
                    var snapshot = new DcimSnapshot
                    {
                        JsonBody = response.GetRawText(),
                        DcimType = request.DcimType,
                        //PageNumber = page,
                        CurrentDate = DateTime.UtcNow
                    };


                    await _repository.AddAsync(
                        snapshot,
                        token);


                    pagesSaved++;


                    _logger.LogInformation(
                        "Saved DCIM page {Page} for {Type}",
                        page,
                        request.DcimType);
                },


                totalPropertyName: "total_count",
                defaultPageSize: 5,
                ct);


            return ApiResponse<object?>.Success(new
            {
                DcimType = request.DcimType,
                PagesSaved = pagesSaved
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed saving DCIM snapshot");


            return ApiResponse<object?>
                .ErrorResponse(
                    "DCIM_ERROR",
                    "Failed to save DCIM snapshot",
                    500);
        }
    }


    private static string GetEndpoint(DcimType type)
    {
        return type switch
        {
            DcimType.VirtualInfrastructure =>
                "/its/portal/vi",

            DcimType.Rack =>
                "/its/portal/rack",

            DcimType.Device =>
                "/its/portal/device",

            DcimType.IP =>
                "/its/portal/ip",

            DcimType.Network =>
                "/its/portal/network",

            _ =>
                throw new ArgumentOutOfRangeException(nameof(type))
        };
    }
}