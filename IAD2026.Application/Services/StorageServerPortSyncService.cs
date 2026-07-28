using System.Text.RegularExpressions;
using IAD2026.Application.Common.Dtos.Storage;
using IAD2026.Application.Interfaces;
using IAD2026.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace IAD2026.Application.Services;

public class SwitchPortSyncService : ISwitchPortSyncService
{
    private readonly IDcimPhysicalUniqueRepository _physicalRepository;
    private readonly IStorageServerPortRepository _storageRepository;
    private readonly IExternalApiClient _externalApiClient;
    private readonly IPaginatedFetcher _paginatedFetcher;
    private readonly ILogger<SwitchPortSyncService> _logger;

    private static readonly Regex HwRegex =
        new("^[0-9A-Fa-f]{16}$", RegexOptions.Compiled);

    public SwitchPortSyncService(
        IDcimPhysicalUniqueRepository physicalRepository,
        IStorageServerPortRepository storageRepository,
        IExternalApiClient externalApiClient,
        IPaginatedFetcher paginatedFetcher,
        ILogger<SwitchPortSyncService> logger)
    {
        _physicalRepository = physicalRepository;
        _storageRepository = storageRepository;
        _externalApiClient = externalApiClient;
        _paginatedFetcher = paginatedFetcher;
        _logger = logger;
    }

    public async Task SyncAsync(CancellationToken cancellationToken)
    {
        var devices = await _physicalRepository.GetAllAsync(cancellationToken);

        _logger.LogInformation(
            "Starting SwitchPort synchronization. Devices: {Count}",
            devices.Count);

        var ports = new Dictionary<string, StorageServerPort>();

        foreach (var device in devices)
        {
            try
            {
                _logger.LogInformation(
                    "Fetching switchports for device {DeviceId}",
                    device.DeviceId);

                var switchPorts =
                    await _paginatedFetcher.FetchAllAsync<SwitchPortDto>(
                        async (page, pageSize, ct) =>
                        {
                            var offset = (page - 1) * pageSize;

                            return await _externalApiClient.GetDynamicAsync(
                                "DCIM",
                                $"/its/portal/switchports/?switch_id={device.DeviceId}&offset={offset}&limit={pageSize}",
                                ct);
                        },
                        itemsPropertyName: "switchports",
                        totalPropertyName: "total_count",
                        defaultPageSize: 1000,
                        ct: cancellationToken);

                foreach (var port in switchPorts)
                {
                    if (string.IsNullOrWhiteSpace(port.HwAddress))
                        continue;

                    if (!HwRegex.IsMatch(port.HwAddress))
                        continue;

                    var key = $"{device.DeviceId}-{port.HwAddress}";

                    ports[key] = new StorageServerPort
                    {
                        HostName = device.Name,
                        DeviceId = device.DeviceId.ToString(),
                        HwAddress = port.HwAddress
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed processing device {DeviceId}",
                    device.DeviceId);
            }
        }

        _logger.LogInformation(
            "Saving {Count} switchports",
            ports.Count);

        await _storageRepository.DeleteAllAsync(cancellationToken);

        await _storageRepository.AddRangeAsync(
            ports.Values.ToList(),
            cancellationToken);

        await _storageRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("SwitchPort synchronization completed.");
    }
}