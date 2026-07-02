using ClosedXML.Excel;
using MasterDataAutomation.Application.Common.Constants;
using MasterDataAutomation.Application.Dtos;
using MasterDataAutomation.Application.Interfaces.Services;
using MasterDataAutomation.Application.Validators;
using MasterDataAutomation.Infrastructure.Excel.Mapping;
using MasterDataAutomation.Infrastructure.Excel.Validators;
using MasterDataAutomation.Application.Common.Helpers;

namespace MasterDataAutomation.Infrastructure.Excel
{
    public class ExcelReaderService : IExcelReaderService
    {
        private readonly ExcelHeaderMapper _headerMapper;
        private readonly ExcelHeaderValidator _headerValidator;
        private readonly ICustomerValidator _customerValidator;
        public ExcelReaderService(ICustomerValidator customerValidator)
        {
            _headerMapper = new ExcelHeaderMapper();
            _headerValidator = new ExcelHeaderValidator();
            _customerValidator = customerValidator;
        }
        public async Task<ExcelReadResult> ReadCustomersAsync(Stream stream)
        {
            var result = new ExcelReadResult();

            using var workbook = new XLWorkbook(stream);

            var worksheet = workbook.Worksheet("Sheet2");

            var headers = _headerMapper.Map(worksheet);

            var headerErrors = _headerValidator.Validate(headers);

            if (headerErrors.Any())
            {
                foreach (var error in headerErrors)
                {
                    result.Errors.Add(new ExcelErrorDto
                    {
                        RowNumber = 1,
                        Column = "Header",
                        Message = error
                    });
                }

                return result;
            }

            var rows = worksheet.RangeUsed().RowsUsed().Skip(1);
            int rowNumber = 2;

            foreach (var row in rows)
            {
                var customer = new CustomerImportDto
                {
                    Line = row.Cell(headers[ExcelHeaders.Line]).GetString().Trim(),
                    Market = row.Cell(headers[ExcelHeaders.Market]).GetString().Trim(),
                    //Branch = row.Cell(headers[ExcelHeaders.Branch]).GetString().Trim(),
                    Branch = BranchNormalizer.Normalize(row.Cell(headers[ExcelHeaders.Branch]).GetString()),
                    SalesDistrict = row.Cell(headers[ExcelHeaders.SalesDistrict]).GetString().Trim(),
                    CustomerType = row.Cell(headers[ExcelHeaders.CustomerType]).GetString().Trim(),
                };

                var errors = _customerValidator.Validate(customer, rowNumber);

                if (errors.Any())
                {
                    result.Errors.AddRange(errors);
                }
                else
                {
                    result.Customers.Add(customer);
                }

                rowNumber++;
            }

            Console.WriteLine($"Customers Count = {result.Customers.Count}");
            Console.WriteLine($"Errors Count = {result.Errors.Count}");

            return await Task.FromResult(result);
        }
    }
}
