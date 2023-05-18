using Microsoft.AspNetCore.Mvc;

namespace BlogV2.Controllers
{
    [ApiController]
    [Route("")]
    public class HomeController : ControllerBase
    {
        [HttpGet("")]
        public IActionResult Get(
            [FromServices] IConfiguration config)
        {
            var env = config.GetValue<string>("Environment");
            return Ok(new
            {
                Environment = env
            });
        }
    }
}