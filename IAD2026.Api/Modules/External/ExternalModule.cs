using Carter;
using IAD2026.Application.Features.External.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;           // ← Add this using

namespace IAD2026.Api.Modules.External;

public class ExternalModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/external")
                       .WithTags("External Systems");

        group.MapGet("/fetch", async (
            [FromServices] IMediator mediator,           
            string systemKey,
            string endpoint,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(new FetchExternalDataQuery(systemKey, endpoint), ct);

            return Results.Json(result, statusCode: result.StatusCode);
        })
        .WithName("FetchExternalData")
        .AllowAnonymous();
    }
}