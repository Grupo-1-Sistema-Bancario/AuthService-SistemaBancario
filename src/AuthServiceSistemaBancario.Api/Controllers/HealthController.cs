using Microsoft.AspNetCore.Mvc;

namespace AuthServiceSistemaBancario.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult GetHealth()
    {
        var response = new
        {
            status = "Healthy",
            timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
            service = "Sistema Bancario Authentication Service"
        };

        return Ok(response);
    }
}