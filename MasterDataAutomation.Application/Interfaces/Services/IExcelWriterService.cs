using MasterDataAutomation.Application.Dtos;

public interface IExcelWriterService
{
    Task<byte[]> GenerateSapFileAsync(List<SapCustomerDto> customers);
}