namespace Allocations.Engine.Grains.Interfaces;

public interface IProviderRegistryGrain : IGrainWithStringKey
{
    Task<bool> IsRegistryInitialised();

    Task<long> GetRegisteredProvidersCount();

    Task<long> Initialise(int size);
}
