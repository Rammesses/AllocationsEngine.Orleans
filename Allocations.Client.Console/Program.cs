using Allocations.Client.Console;
using Allocations.Engine.Grains.Interfaces;
using McMaster.Extensions.CommandLineUtils;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.Configuration;

var host = new HostBuilder()
        .UseOrleansClient(client =>
        {
            client.UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "Allocations";
                });
        })
        .ConfigureLogging(logging => logging.AddConsole())
        .Build();

await host.StartAsync();

//var client = host.Services.GetRequiredService<IClusterClient>();

//// Get the availability of a single provider
//var providerId = Guid.NewGuid();
//var provider = client.GetGrain<IProviderGrain>(providerId);
//var isAvailable = await provider.IsAvailable() ? "available" : "not available";

//Console.WriteLine($"Provider {providerId} is {isAvailable}");

CommandLineApplication.Execute<CommandHandlers>(args);

await host.StopAsync();

Console.ReadLine();