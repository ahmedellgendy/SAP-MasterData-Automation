using MasterDataAutomation.Application.Modules.ProductMaster.Dtos;

namespace MasterDataAutomation.Web.ViewModels.ProductMaster;

public class AccountsUserProductDashboardViewModel
{
    public int DraftCount { get; set; }
    public int SubmittedCount { get; set; }

    public List<ProductRequestDto> RecentDrafts { get; set; } = new();
    public List<ProductRequestDto> RecentSubmitted { get; set; } = new();
}