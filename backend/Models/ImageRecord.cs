using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class ImageRecord
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string FileName { get; set; }
        public string OriginalFileName { get; set; }
        public string ContentType { get; set; }
        public long size { get; set; }

        public string RelativePath { get; set; }

        [Required]
        public string OwnerId { get; set; }
        public User Owner { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        public ICollection<ExtractedText> ExtractedTexts { get; set; }
    }
}