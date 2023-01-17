namespace Allocations.Engine.Grains.StateModels;

public class ProviderRegistryState
{
    public bool IsInitialised { get; set; } = false;
    public HashSet<Guid> RegisteredProviderIDs { get; set; } = new ();
}
