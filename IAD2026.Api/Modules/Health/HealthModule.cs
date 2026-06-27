using Carter;

namespace IAD2026.Api.Modules.Health;

public class HealthModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        // Simple GET endpoint
        app.MapGet("/api/health", () =>
        {
            return Results.Ok(new
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            });
        })
        .WithName("HealthCheck")
        .WithTags("Health")
        .Produces(StatusCodes.Status200OK);
    }
}