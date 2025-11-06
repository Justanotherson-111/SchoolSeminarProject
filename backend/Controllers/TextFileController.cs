using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using backend.DataBase;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TextFileController : ControllerBase
    {
        private readonly AppDbContext _db;

        public TextFileController(AppDbContext db) => _db = db;

        private async Task<User?> GetCurrentUserAsync()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c =>
                c.Type == JwtRegisteredClaimNames.Sub ||
                c.Type == ClaimTypes.NameIdentifier ||
                c.Type == "nameid")?.Value;

            if (!Guid.TryParse(userIdClaim, out var userGuid)) return null;
            return await _db.Users.FirstOrDefaultAsync(u => u.Id == userGuid);

        }

        [HttpGet("{imageId:guid}")]
        public async Task<IActionResult> GetTextFiles(Guid imageId)
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return Unauthorized("User not found");

            var image = await _db.Images
                .Include(i => i.TextFiles)
                .FirstOrDefaultAsync(i => i.Id == imageId && i.UserId == user.Id);

            if (image == null) return NotFound("Image not found or does not belong to user");

            return Ok(image.TextFiles);
        }
    }
}
