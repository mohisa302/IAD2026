using Carter;
using IAD2026.Application.Features.Dcim.Commands;
using IAD2026.Domain.Enums;
using MediatR;


namespace IAD2026.Api.Modules.Dcim;


public class DcimModule : ICarterModule
{

    public void AddRoutes(IEndpointRouteBuilder app)
    {

        var group =
            app.MapGroup("/api/dcim")
               .WithTags("DCIM");

        group.MapPost("/snapshot",
            async (
                DcimType type,
                IMediator mediator,
                CancellationToken ct) =>
            {

                var result =
                    await mediator.Send(
                        new SaveDcimSnapshotCommand(type),
                        ct);


                return Results.Json(
                    result,
                    statusCode: result.StatusCode);
            });

    }

}