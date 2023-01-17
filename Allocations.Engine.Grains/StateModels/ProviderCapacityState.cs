using System;
namespace Allocations.Engine.Grains.StateModels;

public class ProviderCapacityState
{
	public ProviderCapacityState()
	{
	}

	public Guid ProviderId { get; set; }
	public string? Name { get; set; }
	public bool IsAvailable { get; set; }
	public int MonthlyCapacityInPoints { get; set; }
	public int AllocationsThisMonthInPoints { get; set; }
}

