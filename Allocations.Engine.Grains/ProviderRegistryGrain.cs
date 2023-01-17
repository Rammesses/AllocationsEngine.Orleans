using Allocations.Engine.Grains.Interfaces;
using Allocations.Engine.Grains.StateModels;

using Microsoft.Extensions.Logging;

using Orleans.Runtime;

namespace Allocations.Engine.Grains;

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

    public Task<long> GetRegisteredProvidersCount()
    {
        if (_registryState == null)
        {
            return Task.FromResult<long>(5);
        }

        return Task.FromResult(State.RegisteredProviderIDs.LongCount());
    }

    public async Task<long> Initialise(int size)
    {
        var rnd = new Random();

        foreach (var index in Enumerable.Range(0, size))
        {
            var providerId = Guid.NewGuid();

            var providerGrain = GrainFactory.GetGrain<IProviderGrain>(providerId);
            await providerGrain.SetName(Faker.Company.Name());
            await providerGrain.SetMonthlyCapacityInPoints(rnd.Next(100));
            await providerGrain.SetIsAvailable(rnd.Next(100) > 75);

            this.State.RegisteredProviderIDs.Add(providerId);
        }

        this._registryState.State.IsInitialised = true;
        await this._registryState.WriteStateAsync();

        return await this.GetRegisteredProvidersCount();
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
