using ClosedXML.Excel;

namespace MasterDataAutomation.Infrastructure.Excel.Mapping
{
        public class ExcelHeaderMapper
        {
        public Dictionary<string, int> Map(IXLWorksheet worksheet)
        {
            var headers = new Dictionary<string, int>();

            var firstRow = worksheet.FirstRowUsed();

            foreach (var cell in firstRow.CellsUsed())
            {
                headers[cell.GetString().Trim()] = cell.Address.ColumnNumber;
            }

            return headers;
        }
    }
}
