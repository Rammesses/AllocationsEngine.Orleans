using Allocations.Engine.Grains.Interfaces.Models;

using Orleans.Concurrency;

namespace Allocations.Engine.Grains.Interfaces;

public interface IProviderRegistryGrain : IGrainWithStringKey
{
    [AlwaysInterleave]
    Task<bool> IsRegistryInitialised();

    [AlwaysInterleave]
    Task<long> GetRegisteredProvidersCount();

    [AlwaysInterleave] 
    Task<long> Initialise(int size);

    [AlwaysInterleave] 
    Task<PagedPanelMemberSummaryCollection> GetPagedProvidersSummaries(int pageNo, int pageSize);
}
