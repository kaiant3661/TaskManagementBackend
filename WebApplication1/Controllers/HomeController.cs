using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        // Public welcome message
        [HttpGet("Welcome")]
        public IActionResult Welcome()
        {
            return Ok("Welcome to the API. No authentication required.");
        }

        // Public endpoint to fetch some public data
        [HttpGet("GetPublicData")]
        public IActionResult GetPublicData()
        {
            var data = new { Message = "This is some public data available without authentication." };
            return Ok(data);
        }
    }
}
