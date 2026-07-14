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


            int snapshotsSaved = 0;
            var currentDate = DateTime.UtcNow;

            await _fetcher.FetchAllRawAsync(

                // Fetch page
                async (offset, limit, token) =>
                {
                    return await _apiClient.GetDynamicAsync(
                        "DCIM",
                        $"{endpoint}?limit={limit}&offset={offset}",
                        token);
                },


                // Save raw response
                async (response, offset, token) =>
                {
                    var snapshot = new DcimSnapshot
                    {
                        JsonBody = response.GetRawText(),

                        DcimType = request.DcimType,

                        CurrentDate = currentDate,

                        PageSize = 5,

                        PageNumber = offset
                    };


                    await _repository.AddAsync(
                        snapshot,
                        token);


                    snapshotsSaved++;


                    _logger.LogInformation(
                        "Saved DCIM snapshot offset {Offset} for {Type}",
                        offset,
                        request.DcimType);
                },


                totalPropertyName: "total_count",
                defaultPageSize: 5,
                ct: ct);


            return ApiResponse<object?>.Success(new
            {
                DcimType = request.DcimType,
                SnapshotsSaved = snapshotsSaved
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed saving DCIM snapshot for {Type}",
                request.DcimType);


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
            DcimType.VI =>
                "/its/portal/vi",

            _ =>
                throw new ArgumentOutOfRangeException(nameof(type))
        };
    }
}