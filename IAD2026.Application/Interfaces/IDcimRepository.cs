using IAD2026.Domain.Entities;

namespace IAD2026.Application.Interfaces;

public interface IDcimRepository
{
    Task AddAsync(
        DcimSnapshot snapshot,
        CancellationToken ct = default);
}