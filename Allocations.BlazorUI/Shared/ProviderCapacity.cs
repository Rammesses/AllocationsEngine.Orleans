namespace Allocations.BlazorUI.Shared
{
    public class ProviderCapacity
    {
        public DateTime ValidAtDate { get; set; }

        public int CapacityInPoints { get; set; }

        public string? Provider { get; set; }

        public Guid? ProviderId { get; set; }

        public bool IsAvailable { get; set; }
    }
}