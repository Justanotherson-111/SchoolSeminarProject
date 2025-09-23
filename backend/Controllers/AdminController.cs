using backend.DataBase;
using backend.Models;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly AppDbContext _db;
        private readonly IImageService _imageService;
        public AdminController(UserManager<User> userManager, AppDbContext appDbContext, IImageService imageService)
        {
            _userManager = userManager;
            _db = appDbContext;
            _imageService = imageService;
        }

        [HttpGet("users")]
        public IActionResult GetUsers()
        {
            var users = _userManager.Users.Select(u => new { u.Id, u.UserName, u.Email, u.FullName }).ToList();
            return Ok(users);
        }
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var images = _db.ImageRecords.Where(i => i.OwnerId == id).ToList();
            foreach (var img in images)
            {
                await _imageService.DeleteImageAsync(img.RelativePath);
            }

            var res = await _userManager.DeleteAsync(user);
            if (!res.Succeeded)
            {
                return BadRequest(res.Errors);
            }
            return Ok(new { message = "deleted" });
        }
    }
}