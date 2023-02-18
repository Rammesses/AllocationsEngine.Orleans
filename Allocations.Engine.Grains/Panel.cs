using Allocations.Engine.Grains.Interfaces;
using Allocations.Engine.Grains.Interfaces.Models;
using Allocations.Engine.Grains.StateModels;

using Microsoft.Extensions.Logging;

using Orleans.Concurrency;
using Orleans.Runtime;
using Orleans.Serialization.Codecs;

using System.Diagnostics;

namespace Allocations.Engine.Grains;

[StatelessWorker]
public class Panel : Grain<PanelState>, IProviderRegistryGrain
{
    public const string StateEntryName = "providerRegistry";
    public const string StorageName = "providerStore";

    private readonly IPersistentState<PanelState> _registryState;
    private readonly ILogger _logger;

    public Panel(
        [PersistentState(StateEntryName, StorageName)]
        IPersistentState<PanelState> registryState,
        ILogger<Panel> logger
        )
    {
        this._registryState = registryState;
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Rehydrated ProviderRegistryGrain {grainIdentit} with {providerCount} ", this.GetGrainId());

        return base.OnActivateAsync(cancellationToken);

    }

    public async Task<PagedPanelMemberSummaryCollection> GetPagedProvidersSummaries(int pageNo, int pageSize)
    {
        if (await this.IsRegistryInitialised() == false)
            return new PagedPanelMemberSummaryCollection();

        var skip = pageNo * pageSize;
        var allProviders = this.State.RegisteredProviderIDs;

        var stopwatch = Stopwatch.StartNew();

        var summaryTasks = allProviders.Select(providerId =>
        {
            var providerGrain = GrainFactory.GetGrain<IPanelMember>(providerId);
            return providerGrain.GetSummary();
        });

        Task.WaitAll(summaryTasks.OfType<Task>().ToArray());

        var providerSummaries = summaryTasks.Select(t => t.Result).ToArray();

        var slice = providerSummaries.OrderByDescending(s => s.CapacityInPoints).Skip(skip).Take(pageSize);

        stopwatch.Stop();

        var result =new PagedPanelMemberSummaryCollection()
        {
            PageNo = pageNo,
            PageSize = pageSize,
            Items = slice.ToArray()
        };

        _logger.LogInformation("Returned {providerCount} providers from {totalCount} in {duration}ms",
                               result.PageCount, 
                               this.State.RegisteredProviderIDs.Count, 
                               stopwatch.ElapsedMilliseconds);
        return result;
    }

    public async Task<long> GetRegisteredProvidersCount()
    {
        if (await this.IsRegistryInitialised() == false)
            return 0;

        return State.RegisteredProviderIDs.LongCount();
    }

    public async Task<long> Initialise(int size)
    {
        var rnd = new Random();

        var stopwatch = Stopwatch.StartNew();

        var initialisationTasks = Enumerable.Range(0, size)
            .Select(index =>
            {
                var providerId = Guid.NewGuid();
                var providerGrain = GrainFactory.GetGrain<IPanelMember>(providerId);
                return Task.Run<Guid>(async () =>
                {
                    await providerGrain.Initialise(Faker.Company.Name(), rnd.Next(100), (rnd.Next(100) > 75));
                    return providerId;
                });
            });

        Task.WaitAll(initialisationTasks.OfType<Task>().ToArray());

        foreach (var task in initialisationTasks)
            this.State.RegisteredProviderIDs.Add(task.Result);

        this._registryState.State.IsInitialised = true;
        await this._registryState.WriteStateAsync();

        stopwatch.Stop();

        var actualCount = await this.GetRegisteredProvidersCount();
        _logger.LogInformation("Initialised registry with {providerCount} providers in {duration}ms", actualCount, stopwatch.ElapsedMilliseconds);
        return actualCount;
    }

    public Task<bool> IsRegistryInitialised()
    {
        if (_registryState == null)
        {
            return Task.FromResult(false);
        }

        return Task.FromResult(_registryState.RecordExists);
    }
}
