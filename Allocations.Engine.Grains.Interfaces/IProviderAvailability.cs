namespace Allocations.Engine.Grains.Interfaces
{
    public interface IProviderAvailability
    {
        Guid ProviderId {  get; }
        string ProviderName { get; }
        int CurrentCapacity { get; }
        TimeSpan LeadTime { get; }
    }
}