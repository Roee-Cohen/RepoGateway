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
        private readonly ILogger<AuthController> _logger;
        public AuthController(
            ILogger<AuthController> logger,
            UserManager<IdentityUser> userManager,
            IAuthService jwtTokenService
            )
        {
            _logger = logger;
            _userManager = userManager;
            _jwtService = jwtTokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            _logger.LogInformation("Register user {UserName}", model.UserName);
            var user = new IdentityUser { UserName = model.UserName, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                _logger.LogInformation("User {UserName} already registered", model.UserName);
                return BadRequest(result.Errors);
            }

            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            _logger.LogInformation("Login user {UserName}", model.UserName);
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                _logger.LogInformation("User {UserName} does not match credentials", model.UserName);
                return Unauthorized("Invalid credentials");
            }

            var token = _jwtService.GenerateAccessToken(user);
            return Ok(new { token });
        }
    }
}
