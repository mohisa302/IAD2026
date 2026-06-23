using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net.Http.Json;
using Xunit;

public class TasksControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public TasksControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task POST_Task_Should_Return_Ok()
    {
        var response = await _client.PostAsJsonAsync("/api/tasks", new
        {
            title = "Integration test"
        });

        response.EnsureSuccessStatusCode();
    }
}