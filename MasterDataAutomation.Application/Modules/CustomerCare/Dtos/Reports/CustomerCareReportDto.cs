
namespace MasterDataAutomation.Application.Modules.CustomerCare.Dtos.Reports;

public class CustomerCareReportDto
{
    // Applied filters
    public CustomerCareReportFilterDto Filter { get; set; } = new();

    // Overall performance across branches
    public CustomerCareReportSummaryDto Summary { get; set; } = new();

    

    // Complaint categories
    public List<CustomerCareReportBreakdownDto> ByCategory { get; set; } = new();

    // Responsible departments
    public List<CustomerCareReportBreakdownDto> ByDepartment { get; set; } = new();

    // Ticket status distribution
    public List<CustomerCareReportBreakdownDto> ByStatus { get; set; } = new();

    // Ticket priority distribution
    public List<CustomerCareReportBreakdownDto> ByPriority { get; set; } = new();

    // Complaint source distribution
    public List<CustomerCareReportBreakdownDto> BySource { get; set; } = new();
}
