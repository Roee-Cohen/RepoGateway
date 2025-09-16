using Microsoft.Extensions.Caching.Memory;
using RepoGateway.Core.Interfaces;
using RepoGateway.Models;
using StackExchange.Redis;
using System.Text.Json;

namespace RepoGateway.Core.Services
{
    public class RepoService : IRepoService
    {
        private readonly IGithubService _github;
        private readonly IConnectionMultiplexer _redis;

        public RepoService(IGithubService github, IConnectionMultiplexer cache)
        {
            _github = github;
            _redis = cache;
        }

        public async Task<SearchResponseDto> SearchAsync(string query)
        {
            var db = _redis.GetDatabase();
            var cacheKey = $"search:{query}";

            // Check Redis cache
            var cached = await db.StringGetAsync(cacheKey);
            if (!cached.IsNullOrEmpty)
            {
                return JsonSerializer.Deserialize<SearchResponseDto>(cached) ?? new SearchResponseDto();
            }

            // Fetch from GitHub
            var result = await _github.SearchRepositoriesAsync(query);

            // cache for 30 seconds (rate-limiting)
            var strVal = JsonSerializer.Serialize(result);
            await _redis.GetDatabase().StringSetAsync(cacheKey, strVal, TimeSpan.FromSeconds(90));
            
            return result;
        }
    }
}
