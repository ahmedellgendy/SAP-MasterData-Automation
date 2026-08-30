namespace MasterDataAutomation.Domain.Modules.CustomerCare.Entities;

public class CustomerCareAttachment
{
    public int Id { get; set; }

    // =========================================================
    // Ticket
    // =========================================================

    public int TicketId { get; set; }

    public CustomerCareTicket Ticket { get; set; } = null!;

    // =========================================================
    // File
    // =========================================================

    public string FileName { get; set; } = string.Empty;

    public string StoredFileName { get; set; } = string.Empty;

    public string FilePath { get; set; } = string.Empty;

    public string? ContentType { get; set; }

    public long FileSize { get; set; }

    // =========================================================
    // Classification
    // =========================================================

    public string? AttachmentType { get; set; }

    public string? Description { get; set; }

    // =========================================================
    // Audit
    // =========================================================

    public int UploadedByUserId { get; set; }

    public string UploadedByUserName { get; set; } = string.Empty;

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}