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
    public class AllAllowedController : ControllerBase
    {
        [HttpGet("one")]
        public IActionResult One()
        {
            return Ok($"This is message from Controller: AllAllowed, action: {nameof(Two)}");
        }
        [HttpGet("two")]
        public ActionResult<string> Two()
        {
            return Ok($"This is message from Controller: AllAllowed, action: {nameof(Two)}");
        }
        [HttpGet("three")]
        public ActionResult<string> Three()
        {
            return Ok($"This is message from Controller: AllAllowed, action: {nameof(Three)})");
        }
    }
}
