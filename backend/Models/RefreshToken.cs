using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class RefreshToken
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        public string UserId { get; set; } = string.Empty;

        public User User { get; set; } = null!;

        public DateTime Expires { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;

        // When token was revoked (if any)
        public DateTime? Revoked { get; set; }

        // Optional: helps rotation logic
        public string? ReplacedByToken { get; set; }

        // Helper property
        public bool IsExpired => DateTime.UtcNow >= Expires;
        public bool IsActive => Revoked == null && !IsExpired;
    }
}