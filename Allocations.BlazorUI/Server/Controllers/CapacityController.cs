using Allocations.BlazorUI.Shared;
using Allocations.Engine.Grains.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace Allocations.BlazorUI.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CapacityController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly IClusterClient _clusterClient;
        private readonly ILogger<CapacityController> _logger;

        public CapacityController(
            IClusterClient clusterClient,
            ILogger<CapacityController> logger)
        {
            _clusterClient = clusterClient ?? throw new ArgumentNullException(nameof(clusterClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<IEnumerable<ProviderCapacity>> Get()
        {
            var registryGrain = this._clusterClient.GetGrain<IProviderRegistryGrain>("surveyors");
            if ((await registryGrain.IsRegistryInitialised()) == false)
            {
                _logger.LogInformation("Registry is NOT initialised.");
            }

            var registrations = await registryGrain.GetRegisteredProvidersCount();
            return Enumerable.Range(1, (int)registrations).Select(index => new ProviderCapacity
            {
                ValidAtDate = DateTime.Now.AddDays(index),
                CapacityInPoints = Random.Shared.Next(-20, 55),
                Provider = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}