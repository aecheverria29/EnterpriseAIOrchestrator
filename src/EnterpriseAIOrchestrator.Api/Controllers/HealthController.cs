using Microsoft.AspNetCore.Mvc;

namespace EnterpriseAIOrchestrator.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "ok",
            service = "EnterpriseAIOrchestrator.Api",
            utcTime = DateTimeOffset.UtcNow
        });
    }
}
