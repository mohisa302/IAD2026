using IAD2026.Domain.Enums;
using IAD2026.Shared;
using MediatR;

namespace IAD2026.Application.Features.Dcim.Commands;

public record SaveDcimSnapshotCommand(
    DcimType DcimType
) : IRequest<ApiResponse<object?>>;