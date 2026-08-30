namespace MasterDataAutomation.Application.Modules.CustomerCare.Dtos;

public class AddCustomerCareAttachmentDto
{
    public string FileName { get; set; } = string.Empty;

    public string StoredFileName { get; set; } = string.Empty;

    public string FilePath { get; set; } = string.Empty;

    public string? ContentType { get; set; }

    public long FileSize { get; set; }

    public string AttachmentType { get; set; } = "QualityEvidence";

    public string? Description { get; set; }
}