using MasterDataAutomation.Application.Dtos;

namespace MasterDataAutomation.Web.ViewModels;

public class SalesDashboardViewModel
{
    public int DraftCustomersCount { get; set; }

    public int LastBpCode { get; set; }

    public List<CustomerImportDto> RecentCustomers { get; set; } = new();
    public int HistoryCount { get; set; }

    public int SubmittedCount { get; set; }

    public int ApprovedCount { get; set; }

    public int RejectedCount { get; set; }
}