using IAD2026.Domain.Entities;
using IAD2026.Domain.Views;

namespace IAD2026.Application.Interfaces;

public interface IDcimPhysicalUniqueRepository
{
    Task<List<DcimPhysicalUnique>> GetAllAsync(
    CancellationToken cancellationToken);
}