using MasterDataAutomation.Application.Dtos;

namespace MasterDataAutomation.Application.Validators
{
    public interface IDuplicateCustomerValidator
    {
        List<ExcelErrorDto> Validate(List<CustomerImportDto> customers);
    }
}
