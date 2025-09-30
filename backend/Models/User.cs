using Microsoft.AspNetCore.Identity;

namespace backend.Models
{
    public class User : IdentityUser
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public ICollection<ImageRecord> Images { get; set; } = new List<ImageRecord>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}