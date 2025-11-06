using backend.DataBase;
using backend.DTOs;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _db;
        public AdminController(AppDbContext db) => _db = db;

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _db.Users
                .Include(u => u.Images)
                    .ThenInclude(i => i.TextFiles)
                .ToListAsync();

            var dto = users.Select(u => new AdminUserDTO
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                Role = u.Role,
                Images = u.Images.Select(i => new AdminImageDTO
                {
                    Id = i.Id,
                    OriginalFileName = i.OriginalFileName,
                    FileName = i.FileName,
                    Size = i.Size,
                    CreatedAt = i.CreatedAt,
                    TextFiles = i.TextFiles.Select(t => new AdminTextFileDTO
                    {
                        Id = t.Id,
                        Language = t.Language,
                        FileName = Path.GetFileName(t.TxtFilePath), // safe: only file name
                        CreatedAt = t.CreatedAt
                    }).ToList()
                }).ToList()
            });

            return Ok(dto);
        }

        [HttpDelete("user/{id:guid}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _db.Users
                .Include(u => u.Images)
                    .ThenInclude(i => i.TextFiles)
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();

            // Log image & text file deletion
            foreach (var image in user.Images)
            {
                Console.WriteLine($"Deleting Image: {image.OriginalFileName} (Stored: {image.FileName}, Size: {image.Size} bytes)");

                foreach (var txt in image.TextFiles)
                {
                    Console.WriteLine($"   Deleting TextFile: {Path.GetFileName(txt.TxtFilePath)}, Language: {txt.Language}");
                }
            }

            // Delete user (will cascade images, text files, and refresh tokens)
            _db.Users.Remove(user);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
