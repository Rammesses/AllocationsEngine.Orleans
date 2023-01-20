namespace Allocations.Engine.Grains.Interfaces.Models;

[GenerateSerializer]
public struct ProviderSummary
{
    [Id(0)]
    public Guid Id { get; set; }

    [Id(1)]
    public string? Name { get; set; }

    [Id(2)]
    public int CapacityInPoints { get; set; }

    [Id(3)]
    public DateTime? CapacityValidAt { get; set; }
}
