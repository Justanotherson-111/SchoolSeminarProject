using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class TextFile
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(10)]
        public string Language { get; set; } = "eng";

        [Required]
        public string TxtFilePath { get; set; } = string.Empty; // ExtractedText/<username>/

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign key & navigation
        [Required]
        public Guid ImageId { get; set; }
        public Image? Image { get; set; }
    }
}
