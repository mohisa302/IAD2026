using IAD2026.Application.Interfaces;
using Microsoft.Extensions.Logging;
namespace IAD2026.BackgroundJobs.Jobs;

public class SwitchPortSyncJob
{
    private readonly ILogger<SwitchPortSyncJob> _logger;

    public SwitchPortSyncJob(
        ILogger<SwitchPortSyncJob> logger)
    {
        _logger = logger;
    }

    public async Task Execute(CancellationToken ct)
    {
        _logger.LogInformation("Starting SwitchPort sync...");

        _logger.LogInformation("Finished SwitchPort sync.");
    }
}