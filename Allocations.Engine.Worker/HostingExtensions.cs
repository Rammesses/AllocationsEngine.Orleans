namespace Allocations.Engine.Worker;

public static class HostingExtensions
{
    public static bool InDocker => Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
}
