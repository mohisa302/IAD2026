using IAD2026.Domain.Entities;

namespace IAD2026.Application.Interfaces;

public interface IStorageServerPortRepository
{
    Task DeleteAllAsync(CancellationToken cancellationToken);

    Task AddRangeAsync(
        IEnumerable<StorageServerPort> ports,
        CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}