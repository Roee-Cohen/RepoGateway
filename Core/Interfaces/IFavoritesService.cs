using RepoGateway.Models;

namespace RepoGateway.Core.Interfaces
{
    public interface IFavoritesService
    {
        Task<List<Favorite>> GetFavoritesAsync(string userId);
        Task AddFavoriteAsync(string userId, FavoriteDto favorite);
        Task RemoveFavoriteAsync(string userId, string repoId);
    }
}
