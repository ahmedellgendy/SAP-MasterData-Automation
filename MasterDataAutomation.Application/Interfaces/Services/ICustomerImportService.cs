using MasterDataAutomation.Application.Dtos;
using Microsoft.AspNetCore.Http;

namespace MasterDataAutomation.Application.Interfaces.Services;

public interface ICustomerImportService
{
    Task<GenerateResultDto> GenerateSapFileAsync(IFormFile file, int lastBpCode);
    Task<List<CustomerImportDto>> ReadCustomersAsync(IFormFile file);
    Task<GenerateResultDto> GenerateFromDraftAsync();
    Task<ExcelReadResult> ReadCustomersWithErrorsAsync(IFormFile file);

    Task<GenerateResultDto> GenerateFromApprovedAsync();
}