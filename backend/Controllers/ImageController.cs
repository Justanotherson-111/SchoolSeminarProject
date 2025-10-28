using backend.Models;
using backend.Services.Interfaces;
using backend.DataBase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

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
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<ImageController> _logger;

        public ImageController(AppDbContext db, IImageService imageService, UserManager<User> userManager,
            IBackgroundTaskQueue taskQueue, ILogger<ImageController> logger)
        {
            _db = db;
            _imageService = imageService;
            _userManager = userManager;
            _taskQueue = taskQueue;
            _logger = logger;
        }

        [HttpPost("upload")]
        [RequestSizeLimit(70_000_000)]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("No file detected");

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrEmpty(userId)) return Unauthorized();

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
            catch (Exception e)
            {
                _logger.LogError(e, "Upload failed");
                return StatusCode(500, "Upload failed");
            }
        }

        // Keep GET / DELETE / DOWNLOAD logic for images only
    }

}