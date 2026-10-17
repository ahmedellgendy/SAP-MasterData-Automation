
namespace MasterDataAutomation.Application.Modules.CustomerCare.Dtos.Reports;

public class CustomerCareReportFilterDto
{
    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }


    public int? CategoryId { get; set; }

    public int? SubCategoryId { get; set; }

    public int? DepartmentId { get; set; }

    public int? SalesRepId { get; set; }

    public int? CreatedByUserId { get; set; }

    public bool OverdueOnly { get; set; }

    public bool CriticalOnly { get; set; }
}
