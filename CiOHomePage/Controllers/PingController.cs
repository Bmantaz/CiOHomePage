using Microsoft.AspNetCore.Mvc;

namespace CiOHomePage.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PingController : ControllerBase
{
 [HttpGet]
 public IActionResult Get() => Ok(new { message = "pong" });
}
