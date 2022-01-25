using Microsoft.AspNetCore.Mvc;

namespace MaintenanceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartiallyAllowedController : ControllerBase
    {
        [HttpGet("one")]
        public IActionResult One()
        {
            return Ok($"This is message from Controller: PartiallyAllowed, action: {nameof(Two)}");
        }
        [HttpGet("two")]
        public IActionResult Two()
        {
            return Ok($"This is message from Controller: PartiallyAllowed, action: {nameof(Two)}");
        }
        [HttpGet("three")]
        public IActionResult Three()
        {
            return Ok($"This is message from Controller: PartiallyAllowed, action: {nameof(Three)}");
        }
    }
}
