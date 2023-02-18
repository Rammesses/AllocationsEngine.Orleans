using 
    Allocations.BlazorUI.Server;

using Orleans.Configuration;
using Orleans.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddOrleansClient(client =>
{
    Action<ClusterOptions> configureClusterOptions = options =>
    {
        options.ClusterId = "dev";
        options.ServiceId = "Allocations";
    };

    if (HostingExtensions.InDocker)
    {
        client.UseAzureStorageClustering(opts => opts.ConfigureTableServiceClient("UseDevelopmentStorage=true;DevelopmentStorageProxyUri=http://azurite"))
              .Configure(configureClusterOptions);
    }
    else
    {
        client.UseLocalhostClustering()
            .Configure(configureClusterOptions);
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
