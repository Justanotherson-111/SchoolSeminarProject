using backend.DataBase;
using backend.DTOs;
using backend.Models;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IJwtService _jwt;

        public AuthController(AppDbContext db, IJwtService jwt)
        {
            _db = db;
            _jwt = jwt;
        }

        // -------------------------------
        // Registration
        // -------------------------------
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            if (await _db.Users.AnyAsync(u => u.Email == dto.Email))
                return BadRequest(new { message = "Email already exists" });

            var salt = GenerateSalt();
            var user = new User
            {
                UserName = dto.UserName,
                Email = dto.Email,
                Role = "User",
                PasswordSalt = salt,
                PasswordHash = HashPassword(dto.Password, salt)
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            var accessToken = _jwt.GenerateAccessToken(user);
            var refreshToken = _jwt.GenerateRefreshToken(user);

            _db.RefreshTokens.Add(refreshToken);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                accessToken = accessToken,
                refreshToken = refreshToken.Token
            });
        }

        // -------------------------------
        // Login
        // -------------------------------
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            var user = await _db.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null || HashPassword(dto.Password, user.PasswordSalt) != user.PasswordHash)
                return Unauthorized(new { message = "Invalid credentials" });

            // Revoke old tokens
            foreach (var t in user.RefreshTokens.Where(t => !t.Revoked && t.Expires > DateTime.UtcNow))
                t.Revoked = true;

            var accessToken = _jwt.GenerateAccessToken(user);
            var refreshToken = _jwt.GenerateRefreshToken(user);

            _db.RefreshTokens.Add(refreshToken);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                accessToken = accessToken,
                refreshToken = refreshToken.Token
            });
        }

        // -------------------------------
        // Refresh token
        // -------------------------------
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshDTO dto)
        {
            var token = await _db.RefreshTokens
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Token == dto.RefreshToken);

            if (token == null || token.Revoked || token.Expires < DateTime.UtcNow)
                return Unauthorized(new { message = "Invalid or expired refresh token" });

            // Revoke used token
            token.Revoked = true;

            var newAccess = _jwt.GenerateAccessToken(token.User!);
            var newRefresh = _jwt.GenerateRefreshToken(token.User!);

            _db.RefreshTokens.Add(newRefresh);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                accessToken = newAccess,
                refreshToken = newRefresh.Token
            });
        }

        // -------------------------------
        // Utilities: Password Hashing
        // -------------------------------
        public static string GenerateSalt()
        {
            var bytes = new byte[16];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        public static string HashPassword(string password, string salt)
        {
            using var sha = SHA256.Create();
            var combined = Encoding.UTF8.GetBytes(password + salt);
            return Convert.ToBase64String(sha.ComputeHash(combined));
        }
    }
}
