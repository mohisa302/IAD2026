using IAD2026.Application.Interfaces;
using IAD2026.Domain.Entities;

namespace IAD2026.Persistence.Repositories;

public class StorageServerPortRepository : IStorageServerPortRepository
{
    private readonly AppDbContext _context;

    public StorageServerPortRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task DeleteAllAsync(CancellationToken cancellationToken)
    {
        _context.StorageServerPorts.RemoveRange(_context.StorageServerPorts);

        await Task.CompletedTask;
    }

    public async Task AddRangeAsync(
        IEnumerable<StorageServerPort> ports,
        CancellationToken cancellationToken)
    {
        await _context.StorageServerPorts.AddRangeAsync(
            ports,
            cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}