using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace RepoGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        private readonly HealthCheckService _healthCheckService;

        public HealthCheckController(HealthCheckService healthCheckService)
        {
            _healthCheckService = healthCheckService;
        }

        [HttpGet]
        public async Task<IActionResult> GetHealth()
        {
            var report = await _healthCheckService.CheckHealthAsync();

            var res = new
            {
                status = report.Status.ToString(),
                checks = report.Entries.Select(entry => new
                {
                    component = entry.Key,
                    status = entry.Value.Status.ToString(),
                    description = entry.Value.Description,
                }),
                duration = report.TotalDuration.TotalMilliseconds,
            };

            return Ok(res);
        }
    }
}
