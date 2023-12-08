using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace DemonstrateRebusTimeoutIssueApiTests;

public class RebusPublishWithApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public RebusPublishWithApiTests(WebApplicationFactory<Program> factory, ITestOutputHelper outputHelper)
    {
        _factory = factory.WithWebHostBuilder(c => c.ConfigureLogging(cL => cL.AddXUnit(outputHelper)));
    }

    [Fact]
    public async Task Post_Publish_WillThrowTimeoutsOnApi()
    {
        // Arrange
        const int invokeAmount = 1000;
        var client = _factory.CreateClient();

        // Act
        var tasks = Enumerable.Range(0, invokeAmount).Select(_ => client.PostAsync("/test", new StringContent("")));
        var responses = await Task.WhenAll(tasks);
        
        // Assert
        Assert.Equal(invokeAmount, responses.Count(r => r.IsSuccessStatusCode));
    }
}