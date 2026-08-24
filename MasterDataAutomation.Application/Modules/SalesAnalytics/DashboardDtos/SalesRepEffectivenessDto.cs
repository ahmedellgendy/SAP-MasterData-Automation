namespace MasterDataAutomation.Application.Modules.SalesAnalytics.DashboardDtos;

public class SalesRepEffectivenessDto
{
    public int Rank { get; set; }

    public string SalesRepCode { get; set; } = string.Empty;

    public string SalesRepName { get; set; } = string.Empty;

    public string BranchCode { get; set; } = string.Empty;

    public string BranchName { get; set; } = string.Empty;

    // عدد العملاء المختلفين الذين تمت زيارتهم
    public int VisitedCustomers { get; set; }

    // إجمالي عدد الزيارات
    public int TotalVisits { get; set; }

    // الزيارات الإيجابية
    public int PositiveVisits { get; set; }

    // الزيارات السلبية
    public int NegativeVisits { get; set; }

    // Positive Visits / Total Visits
    public decimal PositiveVisitRate { get; set; }

    // Negative Visits / Total Visits
    public decimal NegativeVisitRate { get; set; }

    // Total Visits / Distinct Customers
    public decimal AverageVisitsPerCustomer { get; set; }

    // SuccessfulVisitValue
    public decimal TotalVisitValue { get; set; }

}