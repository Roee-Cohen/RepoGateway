using Microsoft.AspNetCore.Identity;

namespace RepoGateway.Core.Interfaces
{
    public interface IAuthService
    {
        Task<string> GenerateAccessToken(IdentityUser user);
    }
}
