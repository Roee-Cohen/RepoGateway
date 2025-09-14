using Microsoft.EntityFrameworkCore;
using RepoGateway.Core.Data;
using RepoGateway.Core.Interfaces;
using RepoGateway.Models;

namespace RepoGateway.Core.Services
{
    public class FavoritesService : IFavoritesService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IEventPublisher _eventPublisher;

        public FavoritesService(ApplicationDbContext dbContext, IEventPublisher eventPublisher)
        {
            _dbContext = dbContext;
            _eventPublisher = eventPublisher;
        }

        public async Task<List<Favorite>> GetFavoritesAsync(string userId)
        {
            return await _dbContext.Favorites
                .Where(f => f.UserId == userId)
                .ToListAsync();
        }

        public async Task AddFavoriteAsync(string userId, FavoriteDto favorite)
        {
            //if (await _dbContext.Favorites.AnyAsync(f => f.UserId == userId && f.RepoId == favorite.RepoId))
            //    return;

            //var entity = new Favorite
            //{
            //    UserId = userId,
            //    RepoId = favorite.RepoId,
            //    Name = favorite.Name,
            //    Owner = favorite.Owner,
            //    Stars = favorite.Stars,
            //    UpdatedAt = favorite.UpdatedAt
            //};

            //_dbContext.Favorites.Add(entity);
            //await _dbContext.SaveChangesAsync();

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
            var entity = await _dbContext.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.RepoId == repoId);

            if (entity == null)
                return;

            _dbContext.Favorites.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
