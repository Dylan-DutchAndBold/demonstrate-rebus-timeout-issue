using DemonstrateRebusTimeoutIssueTests.TestModels;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DemonstrateRebusTimeoutIssueTests.MassTransit;

public class MassTransitPublishTests : IClassFixture<MassTransitFixture>
{
    private readonly IBus _bus;

    public MassTransitPublishTests()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection
            .AddMassTransitTestHarness(
                x =>
                {
                    x.UsingRabbitMq(
                        (_, cfg) =>
                        {
                            cfg.Host(
                                "localhost",
                                "/",
                                h =>
                                {
                                    h.Username("guest");
                                    h.Password("guest");
                                });
                        });
                });

        var services = serviceCollection.BuildServiceProvider();
        _bus = services.GetRequiredService<IBus>();
    }

    [Fact]
    public async Task PublishMessages_InForLoop_WillNotTakeLong()
    {
        var tasks = new List<Task>();
        for (int i = 0; i < 1000; i++)
        {
            tasks.Add(_bus.Publish(new TestEvent()));
        }

        await Task.WhenAll(tasks);
    }
    
    [Fact]
    public async Task PublishMessages_InParallel_WillNotTakeLong()
    {
        var tasks = new List<Task>();
        Parallel.For(0, 1000, _ => tasks.Add(_bus.Publish(new TestEvent())));
        
        await Task.WhenAll(tasks);
    }
}