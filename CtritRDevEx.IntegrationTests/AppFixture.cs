using Alba;
using Oakton;
using Wolverine;

namespace CtritRDevEx.IntegrationTests;

public class AppFixture : IAsyncLifetime
{
    public IAlbaHost? Host { get; private set; }

    public async Task InitializeAsync()
    {
        OaktonEnvironment.AutoStartHost = true;

        // This is bootstrapping the actual application using
        // its implied Program.Main() set up
        Host = await AlbaHost.For<Program>(x =>
        { 
            // Just showing that you *can* override service
            // registrations for testing if that's useful
            x.ConfigureServices(services =>
            {
                // If wolverine were using Rabbit MQ / SQS / Azure Service Bus,
                // turn that off for now
                services.DisableAllExternalWolverineTransports();
            });

        });
    }

    public async Task DisposeAsync()
    {
        await Host!.StopAsync();
        Host.Dispose();
    }
}