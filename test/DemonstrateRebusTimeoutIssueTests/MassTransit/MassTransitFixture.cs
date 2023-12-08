using DemonstrateRebusTimeoutIssueTests.TestModels;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace DemonstrateRebusTimeoutIssueTests.MassTransit;

public class MassTransitFixture
{
    public MassTransitFixture()
    {
        // Create the queue once to receive the TestEvent
        var serviceCollection = new ServiceCollection();
        serviceCollection
            .AddMassTransitTestHarness(
                x =>
                {
                    x.AddConsumer<TestConsumer>();

                    x.UsingRabbitMq(
                        (context, cfg) =>
                        {
                            cfg.Host(
                                "localhost",
                                "/",
                                h =>
                                {
                                    h.Username("guest");
                                    h.Password("guest");
                                });

                            cfg.ReceiveEndpoint(
                                "demonstrate-rebus-timeout-issue-masstransit",
                                e => { e.ConfigureConsumer<TestConsumer>(context); });
                        });
                });
        var services = serviceCollection.BuildServiceProvider();
        var harness = services.GetRequiredService<ITestHarness>();
        harness.Start().Wait();
        harness.Stop().Wait();
    }

    private class TestConsumer : IConsumer<TestEvent>
    {
        public Task Consume(ConsumeContext<TestEvent> context)
        {
            throw new NotImplementedException();
        }
    }
}