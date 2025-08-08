using KidZone.API.Data;
using KidZone.API.DTOs;
using KidZone.API.Services;
using KidZone.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KidZone.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Parent")]
    public class FavoriteController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;

        public FavoriteController(DataContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }  


        [HttpDelete("remove/{contentId}")]
        public async Task<IActionResult> RemoveFromFavorite(int contentId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserID == userId && f.ContentID == contentId);

            if (favorite == null)
                return NotFound(new { message = "Not found in favorites" });

            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Removed from favorites" });
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToFavorite([FromBody] AddFavoriteDto dto)
        {
            Console.WriteLine("Claims in request:");
            foreach (var c in User.Claims)
                Console.WriteLine($"  {c.Type} = {c.Value}");

            var uid = User.FindFirst("uid")?.Value;

            var nameId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(uid) && !string.IsNullOrEmpty(nameId))
            {
                if (nameId.Contains("@"))
                {
                    var userByEmail = await _userManager.FindByEmailAsync(nameId);
                    if (userByEmail == null)
                        return Unauthorized(new { message = "User from token (email) not found" });

                    uid = userByEmail.Id;
                }
                else
                {
                    uid = nameId; 
                }
            }

            if (string.IsNullOrEmpty(uid))
                return Unauthorized(new { message = "User id not found in token" });

            var user = await _userManager.FindByIdAsync(uid);
            if (user == null)
                return Unauthorized(new { message = "User not found in database" });

            var content = await _context.Contents.FindAsync(dto.ContentID);
            if (content == null)
                return NotFound(new { message = "Content not found" });

            var existingFav = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserID == uid && f.ContentID == dto.ContentID);

            if (existingFav != null)
                return BadRequest(new { message = "Already in favorites" });

            var favorite = new Favorite
            {
                UserID = uid,
                ContentID = dto.ContentID
            };

            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Added to favorites" });
        }

        [HttpGet("my-favorites")]
        public async Task<IActionResult> GetMyFavorites()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine("User ID from token: " + userId);
            var favorites = await _context.Favorites
                .Where(f => f.UserID == userId)
                .Include(f => f.Content)
                .Select(f => new FavoriteDto
                {
                    FavoriteID = f.ID,
                    ContentID = f.ContentID,
                    Title = f.Content.Title,
                    Description = f.Content.Description,
                    ContentType = f.Content.ContentType,
                })
                .ToListAsync();

            return Ok(favorites);
        }
    }


}
