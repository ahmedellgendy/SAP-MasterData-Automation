namespace MasterDataAutomation.Application.Modules.SalesAnalytics.DashboardDtos;

public class BottomCustomerTodayDto
{
    public string CustomerCode { get; set; } = string.Empty;

    public string CustomerName { get; set; } = string.Empty;

    public string SalesRepCode { get; set; } = string.Empty;

    public string SalesRepName { get; set; } = string.Empty;

    public string BranchCode { get; set; } = string.Empty;

    public string BranchName { get; set; } = string.Empty;

    public string SalesDistrictCode { get; set; } = string.Empty;

    public string SalesDistrictName { get; set; } = string.Empty;

    public int TotalVisits { get; set; }

    public int PositiveVisits { get; set; }

    public int NegativeVisits { get; set; }

    public decimal PositiveVisitPercentage { get; set; }

    // إجمالي المبيعات الفعلية للعميل
    public decimal SalesValue { get; set; }

    // إجمالي قيمة الزيارات الناجحة
    public decimal SuccessfulVisitValue { get; set; }

    // أكثر سبب سلبي متكرر
    public string? NegativeReason { get; set; }

    public string? VisitStatus { get; set; }

    // العميل اتزار لكن مفيش أي مبيعات
    public bool HasNoSales { get; set; }

    // العميل كل أو أغلب زياراته سلبية
    public bool RequiresCommercialFollowUp { get; set; }

    // حرج - يحتاج متابعة - متابعة
    public string RiskLevel { get; set; } = string.Empty;
}