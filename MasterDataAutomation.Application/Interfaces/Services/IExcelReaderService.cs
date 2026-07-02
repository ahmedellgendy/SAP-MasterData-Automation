using MasterDataAutomation.Application.Dtos;

namespace MasterDataAutomation.Application.Interfaces.Services
{
    public interface IExcelReaderService
    {
        Task<ExcelReadResult> ReadCustomersAsync(Stream stream);

    }
}
