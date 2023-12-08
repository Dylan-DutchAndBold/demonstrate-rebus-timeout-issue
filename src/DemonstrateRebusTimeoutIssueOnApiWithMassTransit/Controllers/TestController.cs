using DemonstrateRebusTimeoutIssueOnApiWithMassTransit.Models;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace DemonstrateRebusTimeoutIssueOnApiWithMassTransit.Controllers;

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
    public void Publish()
    {
        _bus.Publish(new TestApiEvent());
    }
}