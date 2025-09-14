using Microsoft.AspNetCore.Mvc;
using RepoGateway.Core.Interfaces;

namespace RepoGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RepoController : ControllerBase
    {
        private readonly IRepoService _repoService;

        public RepoController(IRepoService repoService)
        {
            _repoService = repoService;
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return BadRequest("Query is required");

            var result = await _repoService.SearchAsync(q);
            return Ok(result);
        }
    }
}
