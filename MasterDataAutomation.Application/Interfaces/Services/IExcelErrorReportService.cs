using MasterDataAutomation.Application.Dtos;

namespace MasterDataAutomation.Application.Interfaces.Services;

public interface IExcelErrorReportService
{
    Task<byte[]> GenerateAsync(List<ExcelErrorDto> errors);
}