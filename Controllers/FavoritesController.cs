using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepoGateway.Core.Interfaces;
using RepoGateway.Models;
using System.Security.Claims;

namespace RepoGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FavoritesController : ControllerBase
    {
        private readonly IFavoritesService _favoritesService;

        public FavoritesController(IFavoritesService favoritesService)
        {
            _favoritesService = favoritesService;
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        [HttpGet]
        public async Task<IActionResult> GetFavorites()
        {
            var favorites = await _favoritesService.GetFavoritesAsync(GetUserId());
            return Ok(favorites);
        }

        [HttpPost]
        public async Task<IActionResult> AddFavorite([FromBody] FavoriteDto favorite)
        {
            await _favoritesService.AddFavoriteAsync(GetUserId(), favorite);
            return Accepted();
        }

        [HttpDelete("{repoId}")]
        public async Task<IActionResult> RemoveFavorite(string repoId)
        {
            await _favoritesService.RemoveFavoriteAsync(GetUserId(), repoId);
            return NoContent();
        }
    }
}
