using Microsoft.AspNetCore.Mvc;

namespace Beckend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HelloController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetTime()
        {
            var time = DateTime.Now.ToString();
            return Ok(new { time });
        }
    }
}
