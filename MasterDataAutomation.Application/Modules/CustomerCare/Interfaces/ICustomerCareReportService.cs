
using MasterDataAutomation.Application.Modules.CustomerCare.Dtos.Reports;

namespace MasterDataAutomation.Application.Modules.CustomerCare.Interfaces;

public interface ICustomerCareReportService
{
    Task<CustomerCareReportDto> GetReportAsync(
        CustomerCareReportFilterDto filter,
        CancellationToken cancellationToken = default);
}
