using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class ExtractedText
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid ImageRecordId { get; set; }
        public ImageRecord ImageRecord { get; set; }

        [Required]
        public string Text { get; set; }
        public string Language { get; set; } = "eng";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}