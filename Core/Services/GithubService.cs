using System.Net.Http.Headers;
using System.Text.Json;
using RepoGateway.Core.Interfaces;
using RepoGateway.Models;

namespace RepoGateway.Core.Services
{
    public class GithubService : IGithubService
    {
        private readonly HttpClient _httpClient;

        public GithubService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://api.github.com/");
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("RepoGatewayApp");
            var token = configuration["GITHUB_TOKEN"];
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("token", token);
            }
        }

        public async Task<SearchResponseDto> SearchRepositoriesAsync(string query)
        {
            var response = await _httpClient.GetAsync($"search/repositories?q={Uri.EscapeDataString(query)}");

            if (!response.IsSuccessStatusCode)
                return new SearchResponseDto { Items = new List<RepoRo>() };

            using var stream = await response.Content.ReadAsStreamAsync();
            using var jsonDoc = await JsonDocument.ParseAsync(stream);
            var items = jsonDoc.RootElement.GetProperty("items");

            var list = items.EnumerateArray()
                            .Select(item => new RepoRo
                            {
                                RepoId = item.GetProperty("id").GetInt64().ToString(),
                                Name = item.GetProperty("name").GetString(),
                                Owner = item.GetProperty("owner").GetProperty("login").GetString(),
                                Stars = item.GetProperty("stargazers_count").GetInt32(),
                                UpdatedAt = item.GetProperty("updated_at").GetDateTime(),
                                HtmlUrl = item.GetProperty("html_url").GetString()
                            })
                            .ToList();

            return new SearchResponseDto { Items = list };
        }
    }
}
