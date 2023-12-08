using DemonstrateRebusTimeoutIssueTests.TestModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rebus.Activation;
using Rebus.Bus;
using Rebus.Config;
using Xunit.Abstractions;

namespace DemonstrateRebusTimeoutIssueTests.Rebus;

public class RebusPublishTests : IClassFixture<RebusFixture>
{
    private readonly ITestOutputHelper _outputHelper;

    public RebusPublishTests(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task PublishMessages_InForLoop_WillNotTakeLong()
    {
        // Arrange
        var services = CreateServiceProviderWithBus();

        // Act
        var tasks = new List<Task>();
        for (int i = 0; i < 1000; i++)
        {
            using var scope = services.CreateScope();
            tasks.Add(scope.ServiceProvider.GetRequiredService<IBus>().Publish(new TestEvent()));
        }

        await Task.WhenAll(tasks);
    }

    [Fact]
    public async Task PublishMessages_InParallel_IsVerySlow()
    {
        // Arrange
        var services = CreateServiceProviderWithBus();

        // Act
        var tasks = new List<Task>();
        Parallel.For(0, 1000, _ =>
        {
            using var scope = services.CreateScope();
            tasks.Add(scope.ServiceProvider.GetRequiredService<IBus>().Publish(new TestEvent()));
        });

        await Task.WhenAll(tasks);
    }

    [Fact]
    public async Task PublishMessages_InParallelWithoutUsingServiceProvider_IsVerySlow()
    {
        // Arrange
        var bus = CreateBusWithoutServiceProvider();

        // Act
        var tasks = new List<Task>();
        Parallel.For(0, 1000, _ => tasks.Add(bus.Publish(new TestEvent())));

        await Task.WhenAll(tasks);
    }
    
    [Fact]
    public async Task PublishMessages_InParallelUsingHost_IsVerySlow()
    {
        // Arrange
        var host = CreateHostWithBus();
        host.Start();

        // Act
        var tasks = new List<Task>();
        Parallel.For(0, 1000, _ =>
        {
            using var scope = host.Services.CreateScope();
            tasks.Add(scope.ServiceProvider.GetRequiredService<IBus>().Publish(new TestEvent()));
        });

        await Task.WhenAll(tasks);
    }

    private IBus CreateBusWithoutServiceProvider()
    {
        return Configure.With(new BuiltinHandlerActivator())
            .Transport(c => c.UseRabbitMqAsOneWayClient(TestConfiguration.RabbitMqConnectionString))
            .Start();
    }

    private IServiceProvider CreateServiceProviderWithBus()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddRebus(
            c => c.Transport(c => c.UseRabbitMqAsOneWayClient(TestConfiguration.RabbitMqConnectionString)));
        return serviceCollection.BuildServiceProvider();
    }

    private IHost CreateHostWithBus()
    {
        var host = Host
            .CreateDefaultBuilder()
            .ConfigureLogging(c => c.AddXUnit(_outputHelper))
            .ConfigureServices(
                s => s.AddRebus(
                    c => c.Transport(c => c.UseRabbitMqAsOneWayClient(TestConfiguration.RabbitMqConnectionString))));

        return host.Build();
    }
}