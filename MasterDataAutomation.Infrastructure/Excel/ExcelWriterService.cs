using ClosedXML.Excel;
using MasterDataAutomation.Application.Dtos;
using MasterDataAutomation.Application.Interfaces.Services;

namespace MasterDataAutomation.Infrastructure.Excel;

public class ExcelWriterService : IExcelWriterService
{
    public async Task<byte[]> GenerateSapFileAsync(List<SapCustomerDto> customers)
    {
        var templatePath = Path.Combine(
            AppContext.BaseDirectory,
            "Templates",
            "SAP_Template.xlsx");

        if (!File.Exists(templatePath))
        {
            throw new FileNotFoundException(
                $"SAP Template not found: {templatePath}");
        }

        using var workbook = new XLWorkbook(templatePath);

        var worksheet = workbook.Worksheet("BP");


        WriteData(worksheet, customers);

        using var stream = new MemoryStream();

        workbook.SaveAs(stream);

        return await Task.FromResult(stream.ToArray());
    }
    private static void WriteData(IXLWorksheet worksheet, List<SapCustomerDto> customers)
    {
        int row = 2;

        foreach (var customer in customers)
        {
            worksheet.Cell(row, 1).Value = customer.BP_Group;
            worksheet.Cell(row, 2).Value = customer.BP_Code;
            worksheet.Cell(row, 3).Value = customer.Title;
            worksheet.Cell(row, 4).Value = customer.SearchTerm1;
            worksheet.Cell(row, 5).Value = customer.Name1;
            worksheet.Cell(row, 6).Value = customer.Name2;
            worksheet.Cell(row, 7).Value = "";
            worksheet.Cell(row, 8).Value = customer.NielsenId;
            worksheet.Cell(row, 9).Value = customer.Country;
            worksheet.Cell(row, 10).Value = customer.City;
            worksheet.Cell(row, 11).Value = customer.PostalCode1;
            worksheet.Cell(row, 12).Value = customer.Street;
            worksheet.Cell(row, 13).Value = customer.Language;
            worksheet.Cell(row, 14).Value = customer.Region;
            worksheet.Cell(row, 15).Value = customer.TransportZone;
            worksheet.Cell(row, 16).Value = customer.CustAccGroup;
            worksheet.Cell(row, 17).Value = customer.CompanyCode;
            worksheet.Cell(row, 18).Value = customer.SalesOrg;
            worksheet.Cell(row, 19).Value = customer.DistChannel;
            worksheet.Cell(row, 20).Value = customer.Division;
            worksheet.Cell(row, 21).Value = customer.SalesDistrict;
            worksheet.Cell(row, 22).Value = customer.CustomerGroup;
            worksheet.Cell(row, 23).Value = customer.SalesOffice;
            worksheet.Cell(row, 24).Value = customer.PriceList;
            worksheet.Cell(row, 25).Value = customer.CustomerCurrency;
            worksheet.Cell(row, 26).Value = customer.CustPrcGroup;
            worksheet.Cell(row, 27).Value = customer.CustPrcProcedure;
            worksheet.Cell(row, 28).Value = customer.ShippingCond;
            worksheet.Cell(row, 29).Value = customer.Incoterms;
            worksheet.Cell(row, 30).Value = customer.PaymentTerms;
            worksheet.Cell(row, 31).Value = customer.CustGrp1;
            worksheet.Cell(row, 32).Value = customer.DeliveringPlant;
            worksheet.Cell(row, 33).Value = customer.ReconcilAccount;

            row++;
        }

    }
}