using RepoGateway.Models;

namespace RepoGateway.Core.Interfaces
{
    public interface IGithubService
    {
        Task<SearchResponseDto> SearchRepositoriesAsync(string query);
    }
}
