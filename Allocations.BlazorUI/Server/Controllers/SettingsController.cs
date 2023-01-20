using Allocations.BlazorUI.Shared;
using Allocations.Engine.Grains.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace Allocations.BlazorUI.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class SettingsController : ControllerBase
{
    private readonly IClusterClient _clusterClient;
    private readonly ILogger _logger;

    public SettingsController(
        IClusterClient clusterClient,
        ILogger<SettingsController> logger)
    {
        this._clusterClient = clusterClient ?? throw new ArgumentNullException(nameof(clusterClient));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost]
    [Route("initialise")]
    public async Task<IActionResult> Initialise(InitialisationData initialisationData)
    {
        var registryGrain = this._clusterClient.GetGrain<IProviderRegistryGrain>("surveyors");
        //if (await registryGrain.IsRegistryInitialised())
        //{
        //    _logger.LogInformation("Registry is already initialised.");
        //    return (IActionResult)new OkResult();
        //}

        var seedSize = initialisationData.NumberOfProvidersRequired;

        await registryGrain.Initialise(seedSize);

        _logger.LogInformation($"Initialised registry with {seedSize} records.");

        return (IActionResult)new OkResult();
    }
}
