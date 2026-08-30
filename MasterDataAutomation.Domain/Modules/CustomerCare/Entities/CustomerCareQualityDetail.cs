namespace MasterDataAutomation.Domain.Modules.CustomerCare.Entities;

public class CustomerCareQualityDetail
{
    public int Id { get; set; }

    // =========================================================
    // Ticket
    // =========================================================

    public int TicketId { get; set; }

    public CustomerCareTicket Ticket { get; set; } = null!;

    // =========================================================
    // Product
    // =========================================================

    public int? ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string? BatchNumber { get; set; }

    public DateTime? ProductionDate { get; set; }

    public DateTime? ExpiryDate { get; set; }

    // =========================================================
    // Quality Issue
    // =========================================================

    public string? QualityIssueType { get; set; }

    public string? QualityIssueDetails { get; set; }

    public bool SampleRequired { get; set; }

    public bool SampleCollected { get; set; }

    public DateTime? SampleCollectedAt { get; set; }

    // =========================================================
    // Quality Decision
    // =========================================================

    public string? QualityDecision { get; set; }

    public string? CompensationType { get; set; }

    public decimal? CompensationQuantity { get; set; }

    public string? CompensationNotes { get; set; }

    // =========================================================
    // Risk
    // =========================================================

    public bool HasHealthRisk { get; set; }

    public bool HasLegalRisk { get; set; }

    public string? RiskNotes { get; set; }

    // =========================================================
    // Audit
    // =========================================================

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}