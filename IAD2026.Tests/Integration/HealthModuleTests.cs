using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace IAD2026.Tests.Integration;

public class HealthModuleTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public HealthModuleTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GET_Health_Should_Return_Ok_With_Healthy_Status()
    {
        // Act
        var response = await _client.GetAsync("/api/health");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<HealthResponse>();

        Assert.NotNull(content);
        Assert.Equal("Healthy", content.Status);
        Assert.Equal("Development", content.Environment); // Adjust if needed
    }

    private record HealthResponse(
        string Status,
        DateTime Timestamp,
        string Environment
    );
}