using McMaster.Extensions.CommandLineUtils;
using System.Reflection;

namespace Allocations.Client.Console;

[Command(Name = "allocationengine", OptionsComparison = StringComparison.InvariantCultureIgnoreCase)]
[VersionOptionFromMember("--version", MemberName = nameof(GetVersion))]
internal class CommandHandlers
{
    protected Task<int> OnExecute(CommandLineApplication app)
    {
        app.ShowHelp();
        return Task.FromResult(0);
    }

    private static string? GetVersion()
        => typeof(CommandHandlers).Assembly?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

}
