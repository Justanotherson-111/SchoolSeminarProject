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
        private readonly IWebHostEnvironment builder;
        public AuthController(UserManager<User> userManager, IJwtService jwtService
        , RoleManager<IdentityRole> roleManager, AppDbContext dbContext,
        IWebHostEnvironment env)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _roleManager = roleManager;
            _dbContext = dbContext;
            builder = env;
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
            // Debug: check input data (mask password for safety)
            Console.WriteLine($"[DEBUG] Login attempt - Username: {dto.UserName}, Password length: {dto.Password?.Length ?? 0}");

            var user = await _userManager.FindByNameAsync(dto.UserName);
            if (user == null)
            {
                Console.WriteLine($"[DEBUG] User not found: {dto.UserName}");
                return Unauthorized("Invalid username");
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!passwordValid)
            {
                Console.WriteLine($"[DEBUG] Invalid password for user: {dto.UserName}");
                return Unauthorized("Invalid password");
            }

            var roles = await _userManager.GetRolesAsync(user);

            var accessToken = _jwtService.GenerateToken(user, roles);
            var refreshToken = GenerateRToken();

            var refreshEntity = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                Created = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(7),
            };

            _dbContext.RefreshTokens.Add(refreshEntity);
            await _dbContext.SaveChangesAsync();

            Response.Cookies.Append("refreshToken", refreshEntity.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = builder.IsProduction(),
                SameSite = SameSiteMode.None,
                Expires = refreshEntity.Expires
            });

            Console.WriteLine($"[DEBUG] Login successful - User: {user.UserName}, Roles: {string.Join(", ", roles)}");

            return Ok(new { accessToken });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var oldRefreshToken = Request.Cookies["refreshToken"];
            if (oldRefreshToken == null) return Unauthorized(new { error = "No refresh token provided" });

            var oldStoredToken = await _dbContext.RefreshTokens.Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Token == oldRefreshToken);

            if (oldStoredToken == null || oldStoredToken.IsExpired || oldStoredToken.Revoked != null)
                return Unauthorized(new { error = "Invalid or expired refresh token" });
            oldStoredToken.Revoked = DateTime.UtcNow;

            var newToken = new RefreshToken
            {
                Token = GenerateRToken(),
                UserId = oldStoredToken.UserId,
                Expires = DateTime.UtcNow.AddDays(7),
            };
            oldStoredToken.ReplacedByToken = newToken.Token;
            _dbContext.RefreshTokens.Add(newToken);
            await _dbContext.SaveChangesAsync();

            var newAccessToken = _jwtService.GenerateToken(oldStoredToken.User, await _userManager.GetRolesAsync(oldStoredToken.User));
            Response.Cookies.Append("refreshToken", newToken.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = newToken.Expires
            });
            return Ok(new { token = newAccessToken });
        }

        private string GenerateRToken()
        {
            var randomBytes = new Byte[64];
            using var randNum = RandomNumberGenerator.Create();
            randNum.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var refreshToken = Request.Cookies["refreshToken"];
                if (!string.IsNullOrEmpty(refreshToken))
                {
                    // Find token in database and revoke it
                    var storedToken = await _dbContext.RefreshTokens
                        .FirstOrDefaultAsync(r => r.Token == refreshToken);

                    if (storedToken != null)
                    {
                        storedToken.Revoked = DateTime.UtcNow;
                        await _dbContext.SaveChangesAsync();
                    }
                }

                Response.Cookies.Delete("refreshToken");
                return Ok(new { message = "Logged out" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred during logout.");
            }
        }
    }
}