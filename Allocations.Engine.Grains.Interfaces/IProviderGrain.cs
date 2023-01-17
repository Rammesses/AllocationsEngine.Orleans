namespace Allocations.Engine.Grains.Interfaces;

public interface IProviderGrain : Orleans.IGrainWithGuidKey
{
    Task<bool> IsAvailable();
    Task<bool> HasCapacity();
    Task<bool> CanAccept(IWorkDefinition work);
    Task<bool> Allocate(IWorkDefinition work);
    Task SetName(string name);
    Task SetMonthlyCapacityInPoints(int points);
    Task SetIsAvailable(bool isAvailable);
}

