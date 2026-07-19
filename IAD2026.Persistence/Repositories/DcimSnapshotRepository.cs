using IAD2026.Application.Interfaces;
using IAD2026.Domain.Entities;
using IAD2026.Persistence;

namespace IAD2026.Infrastructure.Repositories;

public class DcimRepository : IDcimRepository
{
    private readonly AppDbContext _context;

    public DcimRepository(AppDbContext context)
    {
        _context = context;
    }


    public async Task AddAsync(
        DcimData snapshot,
        CancellationToken ct = default)
    {
        await _context.DcimData.AddAsync(snapshot, ct);

        await _context.SaveChangesAsync(ct);
    }
}