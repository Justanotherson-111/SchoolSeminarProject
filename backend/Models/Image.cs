using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Image
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string FileName { get; set; } = string.Empty; // stored file

        [Required]
        public string OriginalFileName { get; set; } = string.Empty; // uploaded name

        [Required]
        public long Size { get; set; }

        [Required]
        public string RelativePath { get; set; } = string.Empty; // path under Uploads/<username>/

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign key & navigation
        [Required]
        public Guid UserId { get; set; }
        public User? User { get; set; }

        // Navigation property for related TextFiles
        public ICollection<TextFile> TextFiles { get; set; } = new List<TextFile>();
    }
}
