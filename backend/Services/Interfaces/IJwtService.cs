using backend.Models;

namespace backend.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateAccessToken(User user);
        RefreshToken GenerateRefreshToken(User user);
        bool ValidateToken(string token, out string? userId);
    }
}
