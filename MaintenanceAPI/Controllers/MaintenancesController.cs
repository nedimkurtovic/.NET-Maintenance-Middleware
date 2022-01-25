using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaintenanceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaintenancesController : ControllerBase
    {
        [HttpGet("last")]
        public IActionResult Get()
        {
            return Ok();
        }
        [HttpPost()]
        public IActionResult CreateMaintenance()
        {
            return Ok();
        }
        [HttpPatch("{maintenanceId}/disable")]
        public IActionResult DisableMaintenance(int maintenanceId)
        {
            return Ok();
        }
    }
}
