using System.Security.Cryptography;
using backend.DataBase;
using backend.DTOs;
using backend.Models;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Tls;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IJwtService _jwtService;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _dbContext;
        public AuthController(UserManager<User> userManager, IJwtService jwtService, RoleManager<IdentityRole> roleManager, AppDbContext dbContext)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _roleManager = roleManager;
            _dbContext = dbContext;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
            if (await _userManager.FindByNameAsync(dto.UserName) != null)
            {
                return BadRequest("Username already exists.");
            }
            var user = new User { UserName = dto.UserName, Email = dto.Email, FullName = dto.FullName };
            var res = await _userManager.CreateAsync(user, dto.Password);
            if (!res.Succeeded) return BadRequest(res.Errors);

            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (!await _roleManager.RoleExistsAsync("User"))
            {
                await _roleManager.CreateAsync(new IdentityRole("User"));
            }
            return Ok(new { message = "Registered" });
        }
        [EnableRateLimiting("AuthPolicy")]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            var user = await _userManager.FindByNameAsync(dto.UserName);
            if (user == null) return Unauthorized("Invalid credentials");
            if (!await _userManager.CheckPasswordAsync(user, dto.Password)) return Unauthorized("Invalid credentials");

            var roles = await _userManager.GetRolesAsync(user);

            var accessToken = _jwtService.GenerateToken(user, roles);
            var refreshToken = GenerateRToken();
            var refreshEntity = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                Expires = DateTime.Now.AddDays(7),
            };
            _dbContext.RefreshTokens.Add(refreshEntity);
            await _dbContext.SaveChangesAsync();
            Response.Cookies.Append("refreshToken", refreshEntity.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = refreshEntity.Expires
            });
            return Ok(new { accessToken });
        }
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (refreshToken == null) return Unauthorized();

            var storedToken = await _dbContext.RefreshTokens.Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Token == refreshToken);

            if (storedToken == null || storedToken.Expires < DateTime.UtcNow || storedToken.Revoked != null)
                return Unauthorized();

            var roles = await _userManager.GetRolesAsync(storedToken.User);
            var newAccessToken = _jwtService.GenerateToken(storedToken.User, roles);

            return Ok(new { accessToken = newAccessToken });
        }

        private string GenerateRToken()
        {
            var randomBytes = new Byte[64];
            using var randNum = RandomNumberGenerator.Create();
            randNum.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }
}