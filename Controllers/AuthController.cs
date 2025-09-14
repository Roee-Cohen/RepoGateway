using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RepoGateway.Core.Interfaces;
using RepoGateway.Models;

namespace RepoGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IAuthService _jwtService;

        public AuthController(
            UserManager<IdentityUser> userManager,
            IAuthService jwtTokenService
            )
        {
            _userManager = userManager;
            _jwtService = jwtTokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            var user = new IdentityUser { UserName = model.UserName, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return Unauthorized("Invalid credentials");

            var token = await _jwtService.GenerateAccessToken(user);
            return Ok(new { token });
        }
    }
}
