using RepoGateway.Models;

namespace RepoGateway.Core.Interfaces
{
    public interface IRepoService
    {
        public Task<SearchResponseDto> SearchAsync(string query);
    }
}
