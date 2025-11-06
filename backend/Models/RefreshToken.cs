using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class RefreshToken
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        public DateTime Expires { get; set; }

        [Required]
        public bool Revoked { get; set; } = false;

        // Foreign key & navigation
        [Required]
        public Guid UserId { get; set; }

        public User? User { get; set; }
    }
}
