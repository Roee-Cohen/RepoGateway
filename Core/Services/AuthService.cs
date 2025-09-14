

using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using RepoGateway.Core.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RepoGateway.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<IdentityUser> _userManager;

        public AuthService(UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _config = configuration;
        }
        public async Task<string> GenerateAccessToken(IdentityUser user)
        {
            var iss = _config["Jwt:Issuer"];
            var aud = _config["Jwt:Audience"];
            var key = _config["Jwt:Key"];


            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.NameId, user.Id),
                new Claim(JwtRegisteredClaimNames.Name, user.UserName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            };

            var jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(jwtKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: iss,
                audience: aud,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
