namespace Allocations.Engine.Grains.Interfaces.Models;

[GenerateSerializer]
public struct PagedProviderSummaryCollection
{
    public PagedProviderSummaryCollection()
    { }

    [Id(0)]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Id(1)]
    public int PageNo { get; set; }

    [Id(2)]
    public int PageSize { get; set; }

    [Id(3)]
    public ProviderSummary[] Items { get; set; } = Array.Empty<ProviderSummary>();

    public int PageCount => Items.Length;
}
