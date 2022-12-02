using System;
namespace Allocations.Engine.Grains.Interfaces
{
	public interface IWorkDefinition
	{
		Guid ID { get; }
		string Description { get; }
		int Points { get; }
	}
}

