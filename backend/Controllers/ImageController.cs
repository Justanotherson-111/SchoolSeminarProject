using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using backend.DataBase;
using backend.Models;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _images;
        private readonly AppDbContext _db;

        private static readonly Dictionary<Guid, DateTime> _lastRequest = new();
        private static readonly TimeSpan _limit = TimeSpan.FromSeconds(3);

        public ImageController(IImageService images, AppDbContext db)
        {
            _images = images;
            _db = db;
        }

        private async Task<User?> GetCurrentUserAsync()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c =>
                c.Type == JwtRegisteredClaimNames.Sub ||
                c.Type == ClaimTypes.NameIdentifier ||
                c.Type == "nameid")?.Value;

            if (!Guid.TryParse(userIdClaim, out var userGuid)) return null;
            return await _db.Users.FirstOrDefaultAsync(u => u.Id == userGuid);

        }

        private bool CheckRateLimit(User user)
        {
            if (_lastRequest.TryGetValue(user.Id, out var last))
            {
                if ((DateTime.UtcNow - last) < _limit) return false;
            }
            _lastRequest[user.Id] = DateTime.UtcNow;
            return true;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] IFormFile file)
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return Unauthorized("User not found");
            if (!CheckRateLimit(user)) return StatusCode(429, "Rate limit exceeded");
            if (file == null || file.Length == 0) return BadRequest("No file uploaded");

            var image = await _images.UploadImageAsync(file.OpenReadStream(), file.FileName, user);
            return Ok(image);
        }

        [HttpGet]
        public async Task<IActionResult> GetImages()
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return Unauthorized("User not found");

            var images = await _images.GetImagesAsync(user);
            return Ok(images);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return Unauthorized("User not found");

            var success = await _images.DeleteImageAsync(id, user);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
