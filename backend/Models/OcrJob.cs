using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class OcrJob
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid ImageId { get; set; }

        public Image? Image { get; set; }

        public bool Processed { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
