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
public class ProviderRegistryGrain : Grain<ProviderRegistryState>, IProviderRegistryGrain
{
    public const string StateEntryName = "providerRegistry";
    public const string StorageName = "providerStore";

    private readonly IPersistentState<ProviderRegistryState> _registryState;
    private readonly ILogger _logger;

    public ProviderRegistryGrain(
        [PersistentState(StateEntryName, StorageName)]
        IPersistentState<ProviderRegistryState> registryState,
        ILogger<ProviderRegistryGrain> logger
        )
    {
        this._registryState = registryState;
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<PagedProviderSummaryCollection> GetPagedProvidersSummaries(int pageNo, int pageSize)
    {
        if (await this.IsRegistryInitialised() == false)
            return new PagedProviderSummaryCollection();

        var skip = pageNo * pageSize;
        var allProviders = this.State.RegisteredProviderIDs;

        var stopwatch = Stopwatch.StartNew();

        var summaryTasks = allProviders.Select(providerId =>
        {
            var providerGrain = GrainFactory.GetGrain<IProviderGrain>(providerId);
            return providerGrain.GetSummary();
        });

        Task.WaitAll(summaryTasks.OfType<Task>().ToArray());

        var providerSummaries = summaryTasks.Select(t => t.Result).ToArray();

        var slice = providerSummaries.OrderByDescending(s => s.CapacityInPoints).Skip(skip).Take(pageSize);

        stopwatch.Stop();

        var result =new PagedProviderSummaryCollection()
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
                var providerGrain = GrainFactory.GetGrain<IProviderGrain>(providerId);
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
