namespace Allocations.Engine.Grains.Interfaces;

public interface IProviderFilterGrain : Orleans.IGrainWithGuidCompoundKey
{
    Task<IEnumerable<IProviderAvailability>> GetProviderAvailabilities(IWorkDefinition definition);
}
