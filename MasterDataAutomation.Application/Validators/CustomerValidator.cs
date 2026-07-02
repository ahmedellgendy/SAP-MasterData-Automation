using MasterDataAutomation.Application.Dtos;

namespace MasterDataAutomation.Application.Validators
{
    public class CustomerValidator : ICustomerValidator
    {
        private static readonly HashSet<string> ValidBranches =
         new(StringComparer.OrdinalIgnoreCase)
         {
             "10th",
             "Giza",
             "Alex",
             "Mansora"
         };
        private static readonly HashSet<string> ValidCustomerTypes =
        [
            "ثلاجة",
            "خاص"
        ];

        public List<ExcelErrorDto> Validate(CustomerImportDto customer, int rowNumber)
        {
            var errors = new List<ExcelErrorDto>();

            ValidateRequired(customer, rowNumber, errors);

            ValidateBranch(customer, rowNumber, errors);

            ValidateCustomerType(customer, rowNumber, errors);

            return errors;
        }

        private static void ValidateRequired(CustomerImportDto customer, int rowNumber, List<ExcelErrorDto> errors)
        {
            if (string.IsNullOrWhiteSpace(customer.Line))
                errors.Add(new ExcelErrorDto
                {
                    RowNumber = rowNumber,
                    Column = "Line",
                    Message = "Line is required."
                });

            if (string.IsNullOrWhiteSpace(customer.Market))
                errors.Add(new ExcelErrorDto
                {
                    RowNumber = rowNumber,
                    Column = "Market",
                    Message = "Market is required."
                });

            if (string.IsNullOrWhiteSpace(customer.Branch))
                errors.Add(new ExcelErrorDto
                {
                    RowNumber = rowNumber,
                    Column = "Branch",
                    Message = "Branch is required."
                });

            if (string.IsNullOrWhiteSpace(customer.SalesDistrict))
                errors.Add(new ExcelErrorDto
                {
                    RowNumber = rowNumber,
                    Column = "Sales District",
                    Message = "Sales District is required."
                });
        }

        private static void ValidateBranch(CustomerImportDto customer, int rowNumber, List<ExcelErrorDto> errors)
        {
            if (!string.IsNullOrWhiteSpace(customer.Branch)
                && !ValidBranches.Contains(customer.Branch))
            {
                errors.Add(new ExcelErrorDto
                {
                    RowNumber = rowNumber,
                    Column = "Branch",
                    Message = "Invalid Branch."
                });
            }
        }
        private static void ValidateCustomerType(CustomerImportDto customer, int rowNumber, List<ExcelErrorDto> errors)
        {
            if (!string.IsNullOrWhiteSpace(customer.CustomerType)
                && !ValidCustomerTypes.Contains(customer.CustomerType))
            {
                errors.Add(new ExcelErrorDto
                {
                    RowNumber = rowNumber,
                    Column = "Customer Type",
                    Message = "Invalid Customer Type."
                });
            }
        }
    }
}