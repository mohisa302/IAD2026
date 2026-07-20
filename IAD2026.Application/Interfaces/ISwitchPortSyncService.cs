namespace IAD2026.Application.Interfaces;

public interface ISwitchPortSyncService
{
    Task SyncAsync(CancellationToken cancellationToken);
}