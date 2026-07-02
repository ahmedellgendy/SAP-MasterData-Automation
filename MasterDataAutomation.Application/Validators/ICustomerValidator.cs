using MasterDataAutomation.Application.Dtos;

namespace MasterDataAutomation.Application.Validators
{
    public interface ICustomerValidator
    {
        List<ExcelErrorDto> Validate(CustomerImportDto customer, int rowNumber);
    }
}
