using IAD2026.Application.Interfaces;
using Microsoft.Extensions.Logging;
namespace IAD2026.BackgroundJobs.Jobs;

public class SwitchPortSyncJob
{
    private readonly ISwitchPortSyncService _service;
    private readonly ILogger<SwitchPortSyncJob> _logger;

    public SwitchPortSyncJob(
        ISwitchPortSyncService service,
        ILogger<SwitchPortSyncJob> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task Execute(CancellationToken ct)
    {
        _logger.LogInformation("Starting SwitchPort sync...");

        await _service.SyncAsync(ct);

        _logger.LogInformation("Finished SwitchPort sync.");
    }
}