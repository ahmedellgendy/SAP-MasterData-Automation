namespace MasterDataAutomation.Application.Dtos;

public class CustomerReviewDto
{
    public int Id { get; set; }

    public string Line { get; set; } = string.Empty;

    public string Market { get; set; } = string.Empty;

    public string Branch { get; set; } = string.Empty;

    public string SalesDistrict { get; set; } = string.Empty;

    public string CustomerType { get; set; } = string.Empty;
    public string? CreatedBy { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? RejectionReason { get; set; }
}