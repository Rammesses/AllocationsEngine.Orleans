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

                    //siloBuilder.AddMemoryGrainStorageAsDefault()
                    //.AddMemoryGrainStorage(ProviderRegistryGrain.StorageName)
                    //.AddMemoryGrainStorage(ProviderRegistryGrain.StateEntryName);

                        //.AddAzureBlobGrainStorageAsDefault(
                        //    options =>
                        //    {
                        //        options.ConfigureBlobServiceClient("UseDevelopmentStorage=true");
                        //    })
                        //.AddAzureBlobGrainStorage(
                        //    ProviderRegistryGrain.StorageName,
                        //    options =>
                        //    {
                        //        options.ConfigureBlobServiceClient("UseDevelopmentStorage=true");
                        //    });
                })
                .ConfigureServices(s =>
                {
                    s.AddSingleton<IHostedService, Worker>();
                    s.AddOrleans(siloBuilder => {
                        siloBuilder.AddMemoryGrainStorageAsDefault()
                            .AddMemoryGrainStorage(ProviderRegistryGrain.StorageName)
                            .AddMemoryGrainStorage(ProviderRegistryGrain.StateEntryName);
                    });
                })
                .Build();

await host.StartAsync();

Console.WriteLine("\n\n Press Enter to terminate...\n\n");
Console.ReadLine();

await host.StopAsync();