using backend.Models;
using backend.Services.Interfaces;
using backend.DataBase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Iana;
using Microsoft.AspNetCore.RateLimiting;

namespace backend.Controllers
{
    [EnableRateLimiting("UserPolicy")]
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ImageController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IImageService _imageService;
        private readonly IOcrService _ocrService;
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<ImageController> _logger;
        public ImageController(AppDbContext dbContext, IImageService imageService, IOcrService ocrService, UserManager<User> userManager, IBackgroundTaskQueue taskQueue, ILogger<ImageController> logger)
        {
            _db = dbContext;
            _imageService = imageService;
            _ocrService = ocrService;
            _userManager = userManager;
            _taskQueue = taskQueue;
            _logger = logger;
        }
        [HttpPost("upload")]
        [RequestSizeLimit(70_000_000)] // ~ 50MB
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("No file detected");

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                userId = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
                if (string.IsNullOrEmpty(userId)) return Unauthorized();
            }

            try
            {
                var (relPath, savedName) = await _imageService.SaveImageAsync(file);

                var imageRecord = new ImageRecord
                {
                    FileName = savedName,
                    OriginalFileName = file.FileName,
                    ContentType = file.ContentType,
                    size = file.Length,
                    RelativePath = relPath,
                    OwnerId = userId
                };
                _db.ImageRecords.Add(imageRecord);
                await _db.SaveChangesAsync();

                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), relPath);
                await _taskQueue.QueueBackgroundWorkItemAsync(new OcrJob(imageRecord.Id, fullPath, "eng"));

                return Ok(new
                {
                    imageId = imageRecord.Id,
                    path = relPath
                });
            }
            catch (ArgumentException e)
            {
                return BadRequest(new { error = e.Message });
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(new { error = e.Message });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Upload failed");
                return StatusCode(500, "Upload failed");
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetMyImages(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string sort = "uploadedAt_desc",
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null
        )
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                userId = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
            }
            //Get list of user images
            var imgs = _db.ImageRecords.AsNoTracking().Where(i => i.OwnerId == userId);
            //Search image
            if (!string.IsNullOrEmpty(search))
            {
                imgs = imgs.Where(i => i.OriginalFileName.Contains(search));
            }
            //Sorting using datetime
            if (from.HasValue)
            {
                imgs = imgs.Where(i => i.UploadedAt >= from.Value.ToUniversalTime());
            }
            if (to.HasValue)
            {
                imgs = imgs.Where(i => i.UploadedAt <= to.Value.ToUniversalTime());
            }
            //Switch sorting types
            imgs = sort switch
            {
                "uploadedAt_asc" => imgs.OrderBy(i => i.UploadedAt),
                "name_asc" => imgs.OrderBy(i => i.OriginalFileName),
                "name_desc" => imgs.OrderByDescending(i => i.OriginalFileName),
                _ => imgs.OrderByDescending(i => i.UploadedAt)
            };
            var total = await imgs.CountAsync();
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 200);

            var items = await imgs.Skip((page - 1) * pageSize).Take(pageSize)
            .Select(i => new
            {
                i.Id,
                i.OriginalFileName,
                i.RelativePath,
                i.ContentType,
                i.size,
                i.UploadedAt
            }).ToListAsync();

            return Ok(new { page, pageSize, total, items });
        }
        [HttpGet("{id}/text")]
        public async Task<IActionResult> GetExtractedText(Guid guid)
        {
            var txt = await _db.ExtractedTexts.Where(t => t.ImageRecordId == guid).OrderByDescending(t => t.CreatedAt).FirstOrDefaultAsync();
            if (txt == null)
            {
                return NotFound();
            }
            return Ok(new { txt.Id, txt.Text, txt.Language, txt.CreatedAt });
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImage(Guid guid)
        {
            var rec = await _db.ImageRecords.FindAsync(guid);
            if (rec == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                userId = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
            }

            if (rec.OwnerId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }
            await _imageService.DeleteImageAsync(rec.RelativePath);

            _db.ImageRecords.Remove(rec);
            await _db.SaveChangesAsync();
            return Ok(new { message = "deleted" });
        }
        [HttpGet("{id}/download")]
        public async Task<IActionResult> Download(Guid id)
        {
            var rec = await _db.ImageRecords.FindAsync(id);
            if (rec == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                userId = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
            }
            string full;
            if (Path.IsPathRooted(rec.RelativePath))
            {
                full = rec.RelativePath;
            }
            else
            {
                full = Path.Combine(Directory.GetCurrentDirectory(), rec.RelativePath);
            }
            if (!System.IO.File.Exists(full))
            {
                return NotFound();
            }

            var stream = new FileStream(full, FileMode.Open, FileAccess.Read, FileShare.Read);
            return File(stream, rec.ContentType ?? "application/octet-stream", rec.OriginalFileName);

        }
    }
}