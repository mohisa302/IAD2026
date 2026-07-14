using IAD2026.Domain.Enums;

namespace IAD2026.Application.Interfaces;

public interface IDcimService
{
    Task<string> FetchDcimJsonAsync(
        DcimType dcimType,
        CancellationToken ct = default);
}
