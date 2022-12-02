using AllocationServuce;
using Orleans.Configuration;

var host = new HostBuilder()
                .UseOrleans(c =>
                {
                    c.UseLocalhostClustering()
                        .Configure<ClusterOptions>(options =>
                        {
                            options.ClusterId = "dev";
                            options.ServiceId = "Allocations";
                        })
                        .ConfigureLogging(logging => logging.AddConsole());
                })
                .ConfigureServices(s =>
                {
                    s.AddSingleton<IHostedService, Worker>();
                })
    .Build();

await host.StartAsync();

Console.WriteLine("\n\n Press Enter to terminate...\n\n");
Console.ReadLine();

await host.StopAsync();