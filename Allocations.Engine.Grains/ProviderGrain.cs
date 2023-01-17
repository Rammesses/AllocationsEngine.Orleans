using System.Security.Cryptography.X509Certificates;
using Allocations.Engine.Grains.Interfaces;
using Allocations.Engine.Grains.StateModels;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;

namespace Allocations.Engine.Grains;

public class ProviderCapacityGrain : Grain<ProviderCapacityState>, IProviderGrain
{
    public const string StateEntryName = @"ProviderCapacity";
    public const string StorageName = @"Provider";

    private readonly ILogger _logger;
    private readonly bool _isAvailable;

    private readonly IPersistentState<ProviderCapacityState> _registryState;

    public ProviderCapacityGrain(
        [PersistentState(StateEntryName, StorageName)]
        IPersistentState<ProviderCapacityState> registryState, ILogger<ProviderCapacityGrain> logger)
    {
        _registryState = registryState ?? throw new ArgumentNullException(nameof(registryState));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<bool> IsAvailable() => Task.FromResult(State.IsAvailable);

    public Task<bool> Allocate(IWorkDefinition work)
    {
        _logger.LogInformation("Allocated work item {workId} to provider {grainId}", work.ID, this.RuntimeIdentity);
        return Task.FromResult(true);
    }

    public Task<bool> CanAccept(IWorkDefinition work)
    {
        _logger.LogInformation("Requested availability from provider {grainId} for work item {workId}", this.RuntimeIdentity, work.ID);
        return Task.FromResult(true);
    }

    public Task<bool> HasCapacity() => Task.FromResult((State.MonthlyCapacityInPoints - State.AllocationsThisMonthInPoints) > 0);

    public async Task SetName(string name)
    {
        State.Name = name;
        await _registryState.WriteStateAsync();
    }

    public async Task SetMonthlyCapacityInPoints(int points)
    {
        State.MonthlyCapacityInPoints = points;
        await _registryState.WriteStateAsync();
    }

    public async Task SetIsAvailable(bool isAvailable)
    {
        State.IsAvailable = isAvailable;
        await _registryState.WriteStateAsync();
    }
}

