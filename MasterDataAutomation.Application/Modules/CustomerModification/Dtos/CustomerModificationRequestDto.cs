using MasterDataAutomation.Application.Modules.CustomerModification.Enums;

namespace MasterDataAutomation.Application.Modules.CustomerModification.Dtos
{
    public class CustomerModificationRequestDto
    {
        public int Id { get; set; }

        public int BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;

        public string MarketCode { get; set; } = string.Empty;
        public string MarketName { get; set; } = string.Empty;

        public CustomerModificationCustomerType CurrentCustomerType { get; set; }

        public CustomerModificationType ModificationType { get; set; }

        public string? NewMarketName { get; set; }

        public string? Notes { get; set; }

        public CustomerModificationStatus Status { get; set; }

        public string? RejectionReason { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }

        public string? CreatedBy { get; set; }
    }
}
