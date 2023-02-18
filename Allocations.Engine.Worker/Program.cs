using Allocations.Engine.Grains;
using Allocations.Engine.Worker;

using AllocationServuce;

using Orleans.Configuration;

var host = new HostBuilder()
                .UseOrleans(siloBuilder =>
                {
                    Action<ClusterOptions> configureClusterOptions = options =>
                    {
                        options.ClusterId = "dev";
                        options.ServiceId = "Allocations";
                    };

                    if (HostingExtensions.InDocker)
                    {
                        siloBuilder.UseAzureStorageClustering(opts => opts.ConfigureTableServiceClient("UseDevelopmentStorage=true;DevelopmentStorageProxyUri=http://storage"))
                              .Configure(configureClusterOptions);
                    }
                    else
                    {
                        siloBuilder.UseLocalhostClustering()
                            .Configure(configureClusterOptions);
                    }

                    siloBuilder.ConfigureLogging(logging => logging.AddConsole());

                    Action<AzureBlobStorageOptions> configure = (AzureBlobStorageOptions opt) =>
                    {
                    if (HostingExtensions.InDocker)
                            opt.ConfigureBlobServiceClient("UseDevelopmentStorage=true; DevelopmentStorageProxyUri=http://storage");
                        else
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
