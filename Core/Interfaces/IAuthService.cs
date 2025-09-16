using Microsoft.AspNetCore.Identity;

namespace RepoGateway.Core.Interfaces
{
    public interface IAuthService
    {
        string GenerateAccessToken(IdentityUser user);
    }
}
