namespace MasterDataAutomation.Web.ViewModels;

public class BranchStatusSummaryViewModel
{
    public string Branch { get; set; } = string.Empty;

    public int DraftCount { get; set; }

    public int SubmittedCount { get; set; }

    public int ApprovedCount { get; set; }

    public int RejectedCount { get; set; }

    public int TotalCount { get; set; }
}