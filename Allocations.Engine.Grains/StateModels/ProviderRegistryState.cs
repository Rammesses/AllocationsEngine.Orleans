namespace Allocations.Engine.Grains.StateModels;

public class ProviderRegistryState
{
    public IEnumerable<Guid> RegisteredProviderIDs { get; set; } = new List<Guid>();
}
