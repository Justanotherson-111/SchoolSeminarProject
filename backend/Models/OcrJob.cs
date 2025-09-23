namespace backend.Models
{
    public record OcrJob(Guid ImageRecordId, string FullPath, string Language = "eng");
}