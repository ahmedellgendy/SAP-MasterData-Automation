namespace MasterDataAutomation.Application.Modules.CustomerCare.Dtos;

public class CustomerCareQualityDetailDto
{
    public int? ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string? BatchNumber { get; set; }

    public DateTime? ProductionDate { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public string? QualityIssueType { get; set; }

    public string? QualityIssueDetails { get; set; }

    public bool SampleRequired { get; set; }

    public bool SampleCollected { get; set; }

    public DateTime? SampleCollectedAt { get; set; }

    public string? QualityDecision { get; set; }

    public string? CompensationType { get; set; }

    public decimal? CompensationQuantity { get; set; }

    public string? CompensationNotes { get; set; }

    public bool HasHealthRisk { get; set; }

    public bool HasLegalRisk { get; set; }

    public string? RiskNotes { get; set; }
}