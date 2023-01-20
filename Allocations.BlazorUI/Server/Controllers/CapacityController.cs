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

            int pageNo = 0;
            int pageSize = 250;
            var providers = await registryGrain.GetPagedProvidersSummaries(pageNo, pageSize);

            return providers.Items.Select(p => new ProviderCapacity
            {
                ValidAtDate = p.CapacityValidAt ?? DateTime.UtcNow,
                CapacityInPoints = p.CapacityInPoints,
                Provider = p.Name
            })
            .ToArray();
        }
    }
}