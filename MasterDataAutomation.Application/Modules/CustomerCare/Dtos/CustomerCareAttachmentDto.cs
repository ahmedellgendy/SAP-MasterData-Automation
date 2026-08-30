namespace MasterDataAutomation.Application.Modules.CustomerCare.Dtos;

public class CustomerCareAttachmentDto
{
    public int Id { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string FilePath { get; set; } = string.Empty;

    public string? ContentType { get; set; }

    public long FileSize { get; set; }

    public string? AttachmentType { get; set; }

    public string? Description { get; set; }

    public int UploadedByUserId { get; set; }

    public string UploadedByUserName { get; set; } = string.Empty;

    public DateTime UploadedAt { get; set; }
}