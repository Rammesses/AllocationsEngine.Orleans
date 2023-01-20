using Allocations.Engine.Grains.Interfaces.Models;

using Orleans.Concurrency;

namespace Allocations.Engine.Grains.Interfaces;

public interface IProviderGrain : Orleans.IGrainWithGuidKey
{
    [AlwaysInterleave]
    Task<bool> IsAvailable();

    [AlwaysInterleave]
    Task<bool> HasCapacity();

    [AlwaysInterleave]
    Task<bool> CanAccept(IWorkDefinition work);

    Task<bool> Allocate(IWorkDefinition work);

    Task Initialise(string name, int points, bool isAvailable);
    Task SetName(string name);
    Task SetMonthlyCapacityInPoints(int points);
    Task SetIsAvailable(bool isAvailable);

    [AlwaysInterleave]
    Task<ProviderSummary> GetSummary();
}

