using MaintenanceAPI.Models.Requests;
using MaintenanceAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MaintenanceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaintenancesController : ControllerBase
    {
        private readonly IMaintenanceService _maintenanceService;

        public MaintenancesController(IMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService;
        }
        [HttpGet("last")]
        public async Task<IActionResult> Get()
        {
            var lastMaintenance = await _maintenanceService.GetLastMaintenance();
            return Ok(lastMaintenance);
        }
        [HttpGet("endpoints")]
        public async Task<IActionResult> GetAvailableEndpoints()
        {
            var endpoints = await _maintenanceService.GetAvailableEndpoints();
            return Ok(endpoints);
        }
        [HttpPost()]
        public async Task<IActionResult> CreateMaintenance(CreateMaintenanceRequest request)
        {
            await _maintenanceService.CreateMaintenance(request);
            return NoContent();
        }
        [HttpPatch("last/disable")]
        public async Task<IActionResult> DisableLastMaintenance()
        {
            await _maintenanceService.DisableLastMaintenance();
            return NoContent();
        }
    }
}
