using MasterDataAutomation.Application.Modules.ProductMaster.Dtos;

namespace MasterDataAutomation.Web.ViewModels.ProductMaster;

public class AccountsManagerProductDashboardViewModel
{
    public int PendingReviewCount { get; set; }

    public List<ProductRequestDto> RecentPendingRequests { get; set; } = new();
}