using DemonstrateRebusTimeoutIssueOnApiWithMassTransit.Models;
using MassTransit;

namespace DemonstrateRebusTimeoutIssueOnApiWithMassTransit.Consumers;

public class TestApiEventConsumer : IConsumer<TestApiEvent>
{
    public Task Consume(ConsumeContext<TestApiEvent> context)
    {
        return Task.CompletedTask;
    }
}