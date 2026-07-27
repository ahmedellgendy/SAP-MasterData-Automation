using MasterDataAutomation.Application.Modules.ProductMaster.Dtos;

namespace MasterDataAutomation.Web.ViewModels.ProductMaster;

public class ExecutiveProductDashboardViewModel
{
    public int PendingPricingCount { get; set; }

    public List<ProductRequestDto> RecentPricingRequests { get; set; } = new();
}