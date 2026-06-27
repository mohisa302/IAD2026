using Carter;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace IAD2026.Api.Modules.Health;

public class HealthModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/health", ([FromServices] IWebHostEnvironment env) =>
        {
            return Results.Ok(new
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Environment = env.EnvironmentName
            });
        })
        .WithName("HealthCheck")
        .WithTags("Health")
        .Produces(StatusCodes.Status200OK);
    }
}