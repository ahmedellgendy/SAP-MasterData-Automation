using MasterDataAutomation.Application.Dtos;
using MasterDataAutomation.Application.Interfaces.Validators;
using MasterDataAutomation.Application.Validators;

namespace MasterDataAutomation.Infrastructure.Validators
{
    public class DuplicateCustomerValidator : ICustomerImportValidator
    {
        public List<ExcelErrorDto> Validate(List<CustomerImportDto> customers)
        {
            var errors = new List<ExcelErrorDto>();

            var duplicates = customers
                .Select((customer, index) => new
                {
                    Customer = customer,
                    Row = index + 2
                })
                .GroupBy(x => x.Customer.DuplicateKey)
                .Where(g => g.Count() > 1);

            foreach (var group in duplicates)
            {
                foreach (var item in group)
                {
                    errors.Add(new ExcelErrorDto
                    {
                        RowNumber = item.Row,
                        Column = "Customer",
                        Message = "Duplicate customer."
                    });
                }
            }

            return errors;
        }
    }
}
