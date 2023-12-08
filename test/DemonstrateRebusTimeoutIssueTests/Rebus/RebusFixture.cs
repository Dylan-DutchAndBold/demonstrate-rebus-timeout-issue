using DemonstrateRebusTimeoutIssueTests.TestModels;
using Rebus.Activation;
using Rebus.Config;

namespace DemonstrateRebusTimeoutIssueTests.Rebus;

public class RebusFixture
{
    public RebusFixture()
    {
        // Create the queue once to receive the TestEvent
        var bus = Configure.With(new BuiltinHandlerActivator())
            .Transport(c => c.UseRabbitMq(TestConfiguration.RabbitMqConnectionString, "demonstrate-rebus-timeout-issue"))
            .Start();
        bus.Subscribe<TestEvent>().Wait();
        bus.Dispose();
    }
}