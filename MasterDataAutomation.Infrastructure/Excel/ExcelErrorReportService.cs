using ClosedXML.Excel;
using MasterDataAutomation.Application.Dtos;
using MasterDataAutomation.Application.Interfaces.Services;

namespace MasterDataAutomation.Infrastructure.Excel;

public class ExcelErrorReportService : IExcelErrorReportService
{
    public async Task<byte[]> GenerateAsync(List<ExcelErrorDto> errors)
    {
        using var workbook = new XLWorkbook();

        var ws = workbook.Worksheets.Add("Errors");

        ws.Cell(1, 1).Value = "Row";
        ws.Cell(1, 2).Value = "Column";
        ws.Cell(1, 3).Value = "Message";

        int row = 2;

        foreach (var error in errors)
        {
            ws.Cell(row, 1).Value = error.RowNumber;
            ws.Cell(row, 2).Value = error.Column;
            ws.Cell(row, 3).Value = error.Message;

            row++;
        }

        ws.Columns().AdjustToContents();

        using var stream = new MemoryStream();

        workbook.SaveAs(stream);

        return await Task.FromResult(stream.ToArray());
    }
}