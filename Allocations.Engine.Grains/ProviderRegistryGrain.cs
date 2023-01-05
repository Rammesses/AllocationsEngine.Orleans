using Allocations.Engine.Grains.Interfaces;
using Allocations.Engine.Grains.StateModels;

using Microsoft.Extensions.Logging;

using Orleans.Runtime;

namespace Allocations.Engine.Grains;

public class ProviderRegistryGrain : Grain, IProviderRegistryGrain
{
    public const string StateEntryName = "providerRegistry";
    public const string StorageName = "providerStore";

    private readonly IPersistentState<ProviderRegistryState> _registryState = null;
    private readonly ILogger _logger;

    public ProviderRegistryGrain(
        //[PersistentState(StateEntryName, StorageName)]
        //IPersistentState<ProviderRegistryState> registryState,
        ILogger<ProviderRegistryGrain> logger
        )
    {
        //this._registryState = registryState;
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<long> GetRegisteredProvidersCount()
    {
        if (_registryState == null)
        {
            return Task.FromResult<long>(5);
        }

        return Task.FromResult(_registryState.State.RegisteredProviderIDs.LongCount());
    }

    public Task<long> Initialise(int size)
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsRegistryInitialised()
    {
        if (_registryState == null)
        {
            return Task.FromResult(true);
        }

        return Task.FromResult(_registryState.RecordExists);
    }
}
