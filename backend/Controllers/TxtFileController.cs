using backend.DataBase;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TxtFileController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ILogger<TxtFileController> _logger;

        public TxtFileController(AppDbContext db, ILogger<TxtFileController> logger)
        {
            _db = db;
            _logger = logger;
        }

        // GET: api/txtfile/image/{imageId}
        // 🔹 List all text files linked to one image
        [HttpGet("image/{imageId}")]
        public async Task<IActionResult> GetByImage(Guid imageId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var image = await _db.ImageRecords
                .Include(i => i.ExtractedTexts)
                .FirstOrDefaultAsync(i => i.Id == imageId && i.OwnerId == userId);

            if (image == null) return NotFound("Image not found or unauthorized.");

            var results = image.ExtractedTexts.Select(t => new
            {
                t.Id,
                t.Language,
                t.TxtFilePath,
                t.CreatedAt
            });

            return Ok(results);
        }

        // GET: api/txtfile/download/{id}
        // 🔹 Download .txt file
        [HttpGet("download/{id}")]
        public async Task<IActionResult> Download(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var txt = await _db.ExtractedTexts
                .Include(t => t.ImageRecord)
                .FirstOrDefaultAsync(t => t.Id == id && t.ImageRecord.OwnerId == userId);

            if (txt == null || !System.IO.File.Exists(txt.TxtFilePath))
                return NotFound("Text file not found.");

            var fileBytes = await System.IO.File.ReadAllBytesAsync(txt.TxtFilePath);
            var fileName = Path.GetFileName(txt.TxtFilePath);

            return File(fileBytes, "text/plain", fileName);
        }

        // PUT: api/txtfile/edit/{id}
        // 🔹 Edit text content
        [HttpPut("edit/{id}")]
        public async Task<IActionResult> Edit(Guid id, [FromBody] string newText)
        {
            if (string.IsNullOrWhiteSpace(newText))
                return BadRequest("Text cannot be empty.");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var txt = await _db.ExtractedTexts
                .Include(t => t.ImageRecord)
                .FirstOrDefaultAsync(t => t.Id == id && t.ImageRecord.OwnerId == userId);

            if (txt == null || !System.IO.File.Exists(txt.TxtFilePath))
                return NotFound("Text file not found.");

            await System.IO.File.WriteAllTextAsync(txt.TxtFilePath, newText);
            _logger.LogInformation("User {UserId} updated text file {File}", userId, txt.TxtFilePath);

            return Ok("Text file updated successfully.");
        }

        // DELETE: api/txtfile/delete/{id}
        // 🔹 Delete text file and DB record
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var txt = await _db.ExtractedTexts
                .Include(t => t.ImageRecord)
                .FirstOrDefaultAsync(t => t.Id == id && t.ImageRecord.OwnerId == userId);

            if (txt == null)
                return NotFound("Text file not found.");

            if (System.IO.File.Exists(txt.TxtFilePath))
            {
                System.IO.File.Delete(txt.TxtFilePath);
                _logger.LogInformation("Deleted text file from disk: {Path}", txt.TxtFilePath);
            }

            _db.ExtractedTexts.Remove(txt);
            await _db.SaveChangesAsync();

            return Ok("Text file deleted successfully.");
        }
    }
}
