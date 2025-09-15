using RepoGateway.Core.Interfaces;
using RepoGateway.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace RepoGateway.Core.Services
{
    public class FavoritesService : IFavoritesService
    {
        private readonly HttpClient _http;
        private readonly IEventPublisher _eventPublisher;

        public FavoritesService(HttpClient httpClient, IEventPublisher eventPublisher)
        {
            _http = httpClient;
            _http.BaseAddress = new Uri("http://localhost:5002/api/");
            _http.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("RepoAnalyzer", "1.0"));
            _eventPublisher = eventPublisher;
        }

        public async Task<List<Favorite>> GetFavoritesAsync(string userId)
        {
            var res = await _http.GetAsync($"Favorites/{userId}");
            res.EnsureSuccessStatusCode();

            var content = await res.Content.ReadAsStringAsync();
            if (content == null) return [];

            return JsonSerializer.Deserialize<List<Favorite>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? [];
        }

        public async Task AddFavoriteAsync(string userId, FavoriteDto favorite)
        {
            var evt = new RepoFavoritedEvent
            {
                UserId = userId,
                RepoId = favorite.RepoId,
                Name = favorite.Name,
                Owner = favorite.Owner,
                Stars = favorite.Stars,
                UpdatedAt = favorite.UpdatedAt
            };

            await _eventPublisher.PublishRepoFavoritedAsync(evt);
        }

        public async Task RemoveFavoriteAsync(string userId, string repoId)
        {
            var res = await _http.DeleteAsync($"Favorites/{repoId}/{userId}");
            res.EnsureSuccessStatusCode();
        }
    }
}
