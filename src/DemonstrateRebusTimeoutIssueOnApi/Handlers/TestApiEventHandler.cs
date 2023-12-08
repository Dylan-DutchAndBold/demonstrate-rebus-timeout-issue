using DemonstrateRebusTimeoutIssueOnApi.Models;
using Rebus.Handlers;

namespace DemonstrateRebusTimeoutIssueOnApi.Handlers;

public class TestApiEventHandler : IHandleMessages<TestApiEvent>
{
    public Task Handle(TestApiEvent message)
    {
        return Task.CompletedTask;
    }
}