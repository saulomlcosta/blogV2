using Microsoft.AspNetCore.Mvc;

namespace BlogV2.Controllers
{
    [ApiController]
    [Route("")]
    public class HomeController : ControllerBase
    {
        [HttpGet("")]
        public IActionResult Get()
            => Ok();
    }
}