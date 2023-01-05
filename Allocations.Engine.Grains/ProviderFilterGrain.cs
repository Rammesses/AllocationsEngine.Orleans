using Allocations.Engine.Grains.Interfaces;

namespace Allocations.Engine.Grains
{
    public class ProviderFilterGrain : Orleans.Grain, IProviderFilterGrain
    {
        public Task<IEnumerable<IProviderAvailability>> GetProviderAvailabilities(IWorkDefinition definition)
        {
            throw new NotImplementedException();
        }
    }
}
