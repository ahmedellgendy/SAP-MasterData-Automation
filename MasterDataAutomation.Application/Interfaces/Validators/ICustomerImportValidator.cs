using MasterDataAutomation.Application.Dtos;

namespace MasterDataAutomation.Application.Interfaces.Validators;

public interface ICustomerImportValidator
{
    List<ExcelErrorDto> Validate(List<CustomerImportDto> customers);
}