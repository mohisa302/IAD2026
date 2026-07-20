using IAD2026.Application.Interfaces;
using IAD2026.Application.Options;
using IAD2026.Application.Services;
using IAD2026.Domain.Entities;
using IAD2026.Domain.Enums;
using IAD2026.Shared;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IAD2026.Application.Features.Dcim.Commands;

public class SaveDcimSnapshotHandler
    : IRequestHandler<SaveDcimSnapshotCommand, ApiResponse<object?>>
{
    private readonly IExternalApiClient _apiClient;
    private readonly IPaginatedFetcher _fetcher;
    private readonly IDcimRepository _repository;
    private readonly ILogger<SaveDcimSnapshotHandler> _logger;
    private readonly DcimSystemOptions _dcimOptions;


    public SaveDcimSnapshotHandler(
        IExternalApiClient apiClient,
        IPaginatedFetcher fetcher,
        IDcimRepository repository,
        ILogger<SaveDcimSnapshotHandler> logger,
        IOptions<ExternalApiOptions> externalOptions)
    {
        _apiClient = apiClient;
        _fetcher = fetcher;
        _repository = repository;
        _logger = logger;

        _dcimOptions = externalOptions.Value.DCIM;
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
                    var rawJson = response.GetRawText();

                    _logger.LogInformation(
                        "Offset {Offset}, JSON Length = {Length}",
                        offset,
                        rawJson.Length);
                    var snapshot = new DcimData
                    {
                        JsonBody = response.GetRawText(),

                        DcimType = request.DcimType,

                        CurrentDate = currentDate,

                        PageSize = _dcimOptions.Limit,

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

                defaultPageSize: _dcimOptions.Limit,

                ct: ct);



            return ApiResponse<object?>.Success(new
            {
                request.DcimType,
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



    private string GetEndpoint(DcimType type)
    {
        var endpoint = _dcimOptions.Endpoints
            .FirstOrDefault(x => x.DcimType == type);


        if (endpoint is null)
        {
            throw new InvalidOperationException(
                $"No endpoint configured for DCIM type '{type}'.");
        }


        return endpoint.ServicePath;
    }
}