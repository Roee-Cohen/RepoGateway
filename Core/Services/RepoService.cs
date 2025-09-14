using Microsoft.Extensions.Caching.Memory;
using RepoGateway.Core.Interfaces;
using RepoGateway.Models;

namespace RepoGateway.Core.Services
{
    public class RepoService : IRepoService
    {
        private readonly IGithubService _github;
        private readonly IMemoryCache _cache;

        public RepoService(IGithubService github, IMemoryCache cache)
        {
            _github = github;
            _cache = cache;
        }

        public async Task<SearchResponseDto> SearchAsync(string query)
        {
            if (_cache.TryGetValue(query, out SearchResponseDto cached))
                return cached;

            var result = await _github.SearchRepositoriesAsync(query);

            // cache for 30 seconds (rate-limiting)
            _cache.Set(query, result, TimeSpan.FromSeconds(30));
            return result;
        }
    }
}
