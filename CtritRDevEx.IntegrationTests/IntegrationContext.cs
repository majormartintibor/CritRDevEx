﻿using Alba;
using Marten;
using Marten.Storage;
using Microsoft.Extensions.DependencyInjection;
using Wolverine.Runtime;
using Wolverine.Tracking;

namespace CtritRDevEx.IntegrationTests;

[Collection("integration")]
public abstract class IntegrationContext : IAsyncLifetime
{
    private readonly AppFixture _fixture;

    protected IntegrationContext(AppFixture fixture)
    {
        _fixture = fixture;
        Runtime = (WolverineRuntime)fixture.Host!.Services.GetRequiredService<IWolverineRuntime>();
    }

    public WolverineRuntime Runtime { get; }

    public IAlbaHost Host => _fixture.Host!;
    public DocumentStore Store => (DocumentStore)_fixture.Host!.Services.GetRequiredService<IDocumentStore>();

    public virtual async Task InitializeAsync()
    {
        // Using Marten, wipe out all data and reset the state
        // back to exactly what we described in InitialAccountData
        //await Store.Advanced.ResetAllData();
        await ResetAllDataAsync();
    }

    // This is required because of the IAsyncLifetime
    // interface. Note that I do *not* tear down database
    // state after the test. That's purposeful
    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    //can be removed?
    public Task<IScenarioResult> Scenario(Action<Scenario> configure)
    {
        return Host.Scenario(configure);
    }

    // This method allows us to make HTTP calls into our system
    // in memory with Alba, but do so within Wolverine's test support
    // for message tracking to both record outgoing messages and to ensure
    // that any cascaded work spawned by the initial command is completed
    // before passing control back to the calling test
    protected async Task<(ITrackedSession, IScenarioResult)> TrackedHttpCall(Action<Scenario> configuration)
    {
        IScenarioResult result = null!;

        // The outer part is tying into Wolverine's test support
        // to "wait" for all detected message activity to complete
        var tracked = await Host.ExecuteAndWaitAsync(async () =>
        {
            // The inner part here is actually making an HTTP request
            // to the system under test with Alba
            result = await Host.Scenario(configuration);
        });

        return (tracked, result);
    }

    private async Task ResetAllDataAsync(CancellationToken cancellation = default(CancellationToken))
    {
        foreach (IMartenDatabase database in (await Store.Tenancy.BuildDatabases()).OfType<IMartenDatabase>())
        {
            await database.DeleteAllDocumentsAsync(cancellation);           
        }
    }    
}