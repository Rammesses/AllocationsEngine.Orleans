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

                    siloBuilder.AddAzureBlobGrainStorageAsDefault((AzureBlobStorageOptions opt) =>
                    {
                        opt.ConfigureBlobServiceClient("UseDevelopmentStorage=true");
                    });
                    siloBuilder.AddAzureBlobGrainStorage(ProviderRegistryGrain.StorageName);
                    siloBuilder.AddAzureBlobGrainStorage(ProviderCapacityGrain.StorageName);
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