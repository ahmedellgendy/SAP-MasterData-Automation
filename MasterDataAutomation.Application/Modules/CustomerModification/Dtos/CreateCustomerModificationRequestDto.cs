using MasterDataAutomation.Application.Modules.CustomerModification.Enums;

namespace MasterDataAutomation.Application.Modules.CustomerModification.Dtos
{
    public class CreateCustomerModificationRequestDto
    {
        public int? BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;

        public string MarketCode { get; set; } = string.Empty;
        public string MarketName { get; set; } = string.Empty;

        public CustomerModificationCustomerType? CurrentCustomerType { get; set; }

        public CustomerModificationType? ModificationType { get; set; }

        public string? NewMarketName { get; set; }

        public string? Notes { get; set; }
    }
}
