using DemonstrateRebusTimeoutIssueOnApi.Models;
using Microsoft.AspNetCore.Mvc;
using Rebus.Bus;

namespace DemonstrateRebusTimeoutIssueOnApi.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    private readonly IBus _bus;

    public TestController(IBus bus)
    {
        _bus = bus;
    }

    [HttpPost]
    public Task Publish()
    {
        return _bus.Publish(new TestApiEvent());
    }
}