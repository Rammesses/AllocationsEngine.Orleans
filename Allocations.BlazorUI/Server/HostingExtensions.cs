namespace Allocations.BlazorUI.Server;

public static class HostingExtensions
{
    public static bool InDocker => Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
}
