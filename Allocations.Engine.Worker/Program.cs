using Allocations.Engine.Grains;

using AllocationServuce;

using Orleans.Configuration;
using Orleans.Hosting;

var host = new HostBuilder()
                .UseOrleans(siloBuilder =>
                {
                    siloBuilder.UseLocalhostClustering()
                        .Configure<ClusterOptions>(options =>
                        {
                            options.ClusterId = "dev";
                            options.ServiceId = "Allocations";
                        })
                        .ConfigureLogging(logging => logging.AddConsole());

                    Action<AzureBlobStorageOptions> configure = (AzureBlobStorageOptions opt) =>
                    {
                        opt.ConfigureBlobServiceClient("UseDevelopmentStorage=true");
                    };

                    siloBuilder.AddAzureBlobGrainStorageAsDefault(configure);
                    siloBuilder.AddAzureBlobGrainStorage(ProviderRegistryGrain.StorageName, configure);
                    siloBuilder.AddAzureBlobGrainStorage(ProviderCapacityGrain.StorageName, configure);
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