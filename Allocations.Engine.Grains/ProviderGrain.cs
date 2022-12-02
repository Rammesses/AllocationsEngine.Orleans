using Allocations.Engine.Grains.Interfaces;
using Microsoft.Extensions.Logging;

namespace Allocations.Engine.Grains;

public class ProviderCapacityGrain : Orleans.Grain, IProviderGrain
{
    private readonly ILogger _logger;

    public ProviderCapacityGrain(ILogger<ProviderCapacityGrain> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<bool> IsAvailable()
    {
        return Task.FromResult(true);
    }

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

    public Task<bool> HasCapacity()
    {
        _logger.LogInformation("Requested capacity from provider {grainId}", this.RuntimeIdentity);
        return Task.FromResult(true);
    }
}

