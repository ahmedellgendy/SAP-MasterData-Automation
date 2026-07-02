using MasterDataAutomation.Application.Dtos;

namespace MasterDataAutomation.Web.ViewModels;

public class ManagerDashboardViewModel
{
    public int DraftCustomersCount { get; set; }

    public int LastBpCode { get; set; }

    public int HistoryCount { get; set; }

    public Dictionary<string, int> BranchSummary { get; set; } = new();

    public List<GenerationHistoryDto> RecentHistory { get; set; } = new();

    public int SubmittedCount { get; set; }

    public int ApprovedCount { get; set; }

    public int RejectedCount { get; set; }

    public List<BranchStatusSummaryDto> BranchStatusSummary { get; set; } = new();
    public List<CustomerReviewDto> PendingSubmittedCustomers { get; set; } = new();
}