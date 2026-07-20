using IAD2026.Application.Interfaces;
using IAD2026.Domain.Views;
using Microsoft.EntityFrameworkCore;

namespace IAD2026.Persistence.Repositories;

public class DcimPhysicalUniqueRepository
    : IDcimPhysicalUniqueRepository
{
    private readonly AppDbContext _context;

    public DcimPhysicalUniqueRepository(
        AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<DcimPhysicalUnique>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.DcimPhysicalUnique
            .AsNoTracking()
            .ToListAsync(cancellationToken);   
     }
}