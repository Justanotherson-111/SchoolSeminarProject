namespace backend.DTOs
{
    public class LoginDTO
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RefreshDTO
    {
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class RegisterDTO
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class TesseractSettings
    {
        public string TessdataPath { get; set; } = string.Empty;
    }

    public class UploadSettings
    {
        public string ImageFolder { get; set; } = "Uploads";
        public string TextFolder { get; set; } = "ExtractedText";
        public long MaxFileBytes { get; set; } = 70_000_000; // 70MB
    }
    public class AdminUserDTO
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public List<AdminImageDTO> Images { get; set; } = new();
    }
    public class AdminTextFileDTO
    {
        public Guid Id { get; set; }
        public string Language { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty; // just the file name
        public DateTime CreatedAt { get; set; }
    }

    public class AdminImageDTO
    {
        public Guid Id { get; set; }
        public string OriginalFileName { get; set; } = string.Empty;
        public long Size { get; set; }
        public string FileName { get; set; } = string.Empty; // stored file name
        public DateTime CreatedAt { get; set; }
        public List<AdminTextFileDTO> TextFiles { get; set; } = new();
    }
}
