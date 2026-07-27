using ClosedXML.Excel;
using ExcelDataReader;
using MasterDataAutomation.Application.Modules.SalesAnalytics.Dtos;
using MasterDataAutomation.Application.Modules.SalesAnalytics.Enums;
using MasterDataAutomation.Application.Modules.SalesAnalytics.Interfaces;
using System.Text;
using ExcelDataReader;
using System.Data;
using System.Text;

namespace MasterDataAutomation.Infrastructure.Modules.SalesAnalytics.Services;

public class SalesAnalyticsMasterDataImportService : ISalesAnalyticsMasterDataImportService
{
    private readonly ISalesAnalyticsMasterDataRepository _repository;

    public SalesAnalyticsMasterDataImportService(ISalesAnalyticsMasterDataRepository repository)
    {
        _repository = repository;
    }

    public SalesAnalyticsImportResultDto ImportCustomersMaster(
        Stream fileStream,
        string originalFileName,
        string? uploadedBy)
    {
        var uploadBatchId = _repository.CreateUploadBatch(
            SalesAnalyticsUploadFileType.CustomerMaster,
            originalFileName,
            reportDate: null,
            uploadedBy);

        try
        {
            using var workbook = new XLWorkbook(fileStream);

            var worksheet = workbook.Worksheets.FirstOrDefault();

            if (worksheet == null)
            {
                _repository.CompleteUploadBatch(
                    uploadBatchId,
                    SalesAnalyticsUploadStatus.Failed,
                    0,
                    0,
                    0,
                    "Excel file does not contain any worksheets.");

                return new SalesAnalyticsImportResultDto
                {
                    Success = false,
                    Message = "Excel file does not contain any worksheets."
                };
            }

            var headerRow = FindHeaderRow(worksheet);

            if (headerRow == 0)
            {
                _repository.CompleteUploadBatch(
                    uploadBatchId,
                    SalesAnalyticsUploadStatus.Failed,
                    0,
                    0,
                    0,
                    "Could not find required headers: Customer and Name 1.");

                return new SalesAnalyticsImportResultDto
                {
                    Success = false,
                    Message = "Could not find required headers: Customer and Name 1."
                };
            }

            var headers = BuildHeaderMap(worksheet, headerRow);

            var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? headerRow;

            var customers = new List<SalesAnalyticsCustomerImportDto>();

            var totalRows = 0;
            var failedRows = 0;

            for (var row = headerRow + 1; row <= lastRow; row++)
            {
                var customerCode = GetValue(worksheet, row, headers, "Customer");
                var customerName = GetValue(worksheet, row, headers, "Name 1");

                if (string.IsNullOrWhiteSpace(customerCode) &&
                    string.IsNullOrWhiteSpace(customerName))
                {
                    continue;
                }

                totalRows++;

                if (string.IsNullOrWhiteSpace(customerCode) ||
                    string.IsNullOrWhiteSpace(customerName))
                {
                    failedRows++;
                    continue;
                }

                customers.Add(new SalesAnalyticsCustomerImportDto
                {
                    CustomerCode = customerCode,
                    CustomerName = customerName,

                    CustomerAccountGroup = GetValue(worksheet, row, headers, "Customer Account Group"),

                    BranchCode = GetValue(worksheet, row, headers, "SOff."),
                    BranchName = GetValue(worksheet, row, headers, "Sales office"),

                    SalesDistrictCode = GetValue(worksheet, row, headers, "SDst"),
                    SalesDistrictName = GetValue(worksheet, row, headers, "Sales District"),

                    CustomerClassificationCode = GetValue(worksheet, row, headers, "CClf"),
                    CustomerClassificationName = GetValue(worksheet, row, headers, "Customer Classification"),

                    IncotermsCode = GetValue(worksheet, row, headers, "IncoT"),
                    IncotermsName = GetValue(worksheet, row, headers, "Incoterms (Part 1)"),

                    SearchTerm = GetValue(worksheet, row, headers, "SEARCH TERM"),
                    SearchTerm2 = GetValue(worksheet, row, headers, "SEARCH TERM 2"),

                    SourceCreatedDate = GetDateValue(worksheet, row, headers, "Date"),
                    SourceCreatedBy = GetValue(worksheet, row, headers, "Created by")
                });
            }

            customers = customers
                .GroupBy(x => x.CustomerCode)
                .Select(x => x.First())
                .ToList();

            _repository.ReplaceCustomers(customers, uploadBatchId);

            var status = failedRows > 0
                ? SalesAnalyticsUploadStatus.PartiallyImported
                : SalesAnalyticsUploadStatus.Success;

            _repository.CompleteUploadBatch(
                uploadBatchId,
                status,
                totalRows,
                customers.Count,
                failedRows);

            return new SalesAnalyticsImportResultDto
            {
                Success = true,
                TotalRows = totalRows,
                ImportedRows = customers.Count,
                FailedRows = failedRows,
                Message = $"Customers master imported successfully. Imported: {customers.Count}, Failed: {failedRows}"
            };
        }
        catch (Exception ex)
        {
            _repository.CompleteUploadBatch(
                uploadBatchId,
                SalesAnalyticsUploadStatus.Failed,
                0,
                0,
                0,
                ex.Message);

            return new SalesAnalyticsImportResultDto
            {
                Success = false,
                Message = ex.Message
            };
        }
    }

    public SalesAnalyticsImportResultDto ImportSalesRepsMaster(
    Stream fileStream,
    string originalFileName,
    string? uploadedBy)
    {
        var uploadBatchId = _repository.CreateUploadBatch(
            SalesAnalyticsUploadFileType.SalesRepMaster,
            originalFileName,
            reportDate: null,
            uploadedBy);

        try
        {
            using var workbook = new XLWorkbook(fileStream);

            var worksheet = workbook.Worksheets.FirstOrDefault();

            if (worksheet == null)
            {
                _repository.CompleteUploadBatch(
                    uploadBatchId,
                    SalesAnalyticsUploadStatus.Failed,
                    0,
                    0,
                    0,
                    "Excel file does not contain any worksheets.");

                return new SalesAnalyticsImportResultDto
                {
                    Success = false,
                    Message = "Excel file does not contain any worksheets."
                };
            }

            var headerRow = FindHeaderRow(worksheet);

            if (headerRow == 0)
            {
                _repository.CompleteUploadBatch(
                    uploadBatchId,
                    SalesAnalyticsUploadStatus.Failed,
                    0,
                    0,
                    0,
                    "Could not find required headers: Customer and Name 1.");

                return new SalesAnalyticsImportResultDto
                {
                    Success = false,
                    Message = "Could not find required headers: Customer and Name 1."
                };
            }

            var headers = BuildHeaderMap(worksheet, headerRow);

            var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? headerRow;

            var salesReps = new List<SalesAnalyticsSalesRepImportDto>();

            var totalRows = 0;
            var failedRows = 0;

            for (var row = headerRow + 1; row <= lastRow; row++)
            {
                var salesRepCode = GetValue(worksheet, row, headers, "Customer");
                var salesRepName = GetValue(worksheet, row, headers, "Name 1");

                if (string.IsNullOrWhiteSpace(salesRepCode) &&
                    string.IsNullOrWhiteSpace(salesRepName))
                {
                    continue;
                }

                totalRows++;

                if (string.IsNullOrWhiteSpace(salesRepCode) ||
                    string.IsNullOrWhiteSpace(salesRepName))
                {
                    failedRows++;
                    continue;
                }

                salesReps.Add(new SalesAnalyticsSalesRepImportDto
                {
                    SalesRepCode = salesRepCode,
                    SalesRepName = salesRepName,

                    BranchCode = GetValue(worksheet, row, headers, "SOff."),
                    BranchName = GetValue(worksheet, row, headers, "Sales office"),

                    RegionCode = GetValue(worksheet, row, headers, "Rg"),
                    RegionName = GetValue(worksheet, row, headers, "Region (State, Province, County)"),

                    InternalCode = GetValue(worksheet, row, headers, "SEARCH TERM"),
                    SearchTerm2 = GetValue(worksheet, row, headers, "SEARCH TERM 2"),

                    SourceCreatedDate = GetDateValue(worksheet, row, headers, "Date"),
                    SourceCreatedBy = GetValue(worksheet, row, headers, "Created by")
                });
            }

            salesReps = salesReps
                .GroupBy(x => x.SalesRepCode)
                .Select(x => x.First())
                .ToList();

            _repository.ReplaceSalesReps(salesReps, uploadBatchId);

            var status = failedRows > 0
                ? SalesAnalyticsUploadStatus.PartiallyImported
                : SalesAnalyticsUploadStatus.Success;

            _repository.CompleteUploadBatch(
                uploadBatchId,
                status,
                totalRows,
                salesReps.Count,
                failedRows);

            return new SalesAnalyticsImportResultDto
            {
                Success = true,
                TotalRows = totalRows,
                ImportedRows = salesReps.Count,
                FailedRows = failedRows,
                Message = $"Sales reps master imported successfully. Imported: {salesReps.Count}, Failed: {failedRows}"
            };
        }
        catch (Exception ex)
        {
            _repository.CompleteUploadBatch(
                uploadBatchId,
                SalesAnalyticsUploadStatus.Failed,
                0,
                0,
                0,
                ex.Message);

            return new SalesAnalyticsImportResultDto
            {
                Success = false,
                Message = ex.Message
            };
        }
    }

    public SalesAnalyticsImportResultDto ImportDailySalesReport(
    Stream fileStream,
    string originalFileName,
    DateTime reportDate,
    string? uploadedBy)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        var uploadBatchId = _repository.CreateUploadBatch(
            SalesAnalyticsUploadFileType.DailySalesReport,
            originalFileName,
            reportDate.Date,
            uploadedBy);

        try
        {
            using var reader = ExcelReaderFactory.CreateReader(fileStream);

            var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration
            {
                ConfigureDataTable = _ => new ExcelDataTableConfiguration
                {
                    UseHeaderRow = false
                }
            });

            var table = dataSet.Tables
                .Cast<DataTable>()
                .FirstOrDefault(x => x.TableName.Equals("Sheet2", StringComparison.OrdinalIgnoreCase))
                ?? dataSet.Tables.Cast<DataTable>().Skip(1).FirstOrDefault()
                ?? dataSet.Tables.Cast<DataTable>().FirstOrDefault();

            if (table == null)
            {
                _repository.CompleteUploadBatch(
                    uploadBatchId,
                    SalesAnalyticsUploadStatus.Failed,
                    0,
                    0,
                    0,
                    "Excel file does not contain any sheets.");

                return new SalesAnalyticsImportResultDto
                {
                    Success = false,
                    Message = "Excel file does not contain any sheets."
                };
            }

            var headerRowIndex = FindDailySalesHeaderRow(table);

            if (headerRowIndex == -1)
            {
                _repository.CompleteUploadBatch(
                    uploadBatchId,
                    SalesAnalyticsUploadStatus.Failed,
                    0,
                    0,
                    0,
                    "Could not find required headers: الكود, الخط, الكمية, اسم الصنف, القيمة.");

                return new SalesAnalyticsImportResultDto
                {
                    Success = false,
                    Message = "Could not find required headers: الكود, الخط, الكمية, اسم الصنف, القيمة."
                };
            }

            var headers = BuildDataTableHeaderMap(table, headerRowIndex);

            var salesRows = new List<SalesAnalyticsDailySalesImportDto>();

            var totalRows = 0;
            var failedRows = 0;

            for (var row = headerRowIndex + 1; row < table.Rows.Count; row++)
            {
                var lineCode = GetTableValue(table, row, headers, "الكود");
                var lineName = GetTableValue(table, row, headers, "الخط");
                var productName = GetTableValue(table, row, headers, "اسم الصنف");

                var quantityText = GetTableValue(table, row, headers, "الكمية");
                var salesAmountText = GetTableValue(table, row, headers, "القيمة");

                if (string.IsNullOrWhiteSpace(lineCode) &&
                    string.IsNullOrWhiteSpace(lineName) &&
                    string.IsNullOrWhiteSpace(productName))
                {
                    continue;
                }

                totalRows++;

                var quantity = ParseDecimal(quantityText);
                var salesAmount = ParseDecimal(salesAmountText);

                if (string.IsNullOrWhiteSpace(lineCode) ||
                    string.IsNullOrWhiteSpace(lineName) ||
                    string.IsNullOrWhiteSpace(productName))
                {
                    failedRows++;
                    continue;
                }

                salesRows.Add(new SalesAnalyticsDailySalesImportDto
                {
                    ReportDate = reportDate.Date,
                    LineCode = lineCode,
                    LineName = lineName,
                    ProductName = productName,
                    Quantity = quantity,
                    SalesAmount = salesAmount
                });
            }

            _repository.ReplaceDailySalesReport(salesRows, uploadBatchId, reportDate.Date);

            var status = failedRows > 0
                ? SalesAnalyticsUploadStatus.PartiallyImported
                : SalesAnalyticsUploadStatus.Success;

            _repository.CompleteUploadBatch(
                uploadBatchId,
                status,
                totalRows,
                salesRows.Count,
                failedRows);

            return new SalesAnalyticsImportResultDto
            {
                Success = true,
                TotalRows = totalRows,
                ImportedRows = salesRows.Count,
                FailedRows = failedRows,
                Message = $"Daily sales report imported successfully. Imported: {salesRows.Count}, Failed: {failedRows}"
            };
        }
        catch (Exception ex)
        {
            _repository.CompleteUploadBatch(
                uploadBatchId,
                SalesAnalyticsUploadStatus.Failed,
                0,
                0,
                0,
                ex.Message);

            return new SalesAnalyticsImportResultDto
            {
                Success = false,
                Message = ex.Message
            };
        }
    }

    public SalesAnalyticsImportResultDto ImportDailyVisitsReport(
    Stream fileStream,
    string originalFileName,
    DateTime reportDate,
    string? uploadedBy)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        var uploadBatchId = _repository.CreateUploadBatch(
            SalesAnalyticsUploadFileType.DailyVisitsReport,
            originalFileName,
            reportDate.Date,
            uploadedBy);

        try
        {
            using var reader = ExcelReaderFactory.CreateReader(fileStream);

            var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration
            {
                ConfigureDataTable = _ => new ExcelDataTableConfiguration
                {
                    UseHeaderRow = false
                }
            });

            var table = dataSet.Tables
                .Cast<DataTable>()
                .FirstOrDefault(x => x.TableName.Equals("Sheet2", StringComparison.OrdinalIgnoreCase))
                ?? dataSet.Tables.Cast<DataTable>().Skip(1).FirstOrDefault()
                ?? dataSet.Tables.Cast<DataTable>().FirstOrDefault();

            if (table == null)
            {
                _repository.CompleteUploadBatch(
                    uploadBatchId,
                    SalesAnalyticsUploadStatus.Failed,
                    0,
                    0,
                    0,
                    "Excel file does not contain any sheets.");

                return new SalesAnalyticsImportResultDto
                {
                    Success = false,
                    Message = "Excel file does not contain any sheets."
                };
            }

            var headerRowIndex = FindDailyVisitsHeaderRow(table);

            if (headerRowIndex == -1)
            {
                _repository.CompleteUploadBatch(
                    uploadBatchId,
                    SalesAnalyticsUploadStatus.Failed,
                    0,
                    0,
                    0,
                    "Could not find required visits report headers.");

                return new SalesAnalyticsImportResultDto
                {
                    Success = false,
                    Message = "Could not find required headers: كود المندوب, اسم المندوب, كود العميل, اسم العميل, حالة الزيارة."
                };
            }

            var headers = BuildDataTableHeaderMap(table, headerRowIndex);

            var visitRows = new List<SalesAnalyticsDailyVisitImportDto>();

            var totalRows = 0;
            var failedRows = 0;

            for (var row = headerRowIndex + 1; row < table.Rows.Count; row++)
            {
                var salesRepCode = GetTableValue(table, row, headers, "كود المندوب");
                var salesRepName = GetTableValue(table, row, headers, "اسم المندوب");

                var customerCode = GetTableValue(table, row, headers, "كود العميل");
                var customerName = GetTableValue(table, row, headers, "اسم العميل");

                if (string.IsNullOrWhiteSpace(salesRepCode) &&
                    string.IsNullOrWhiteSpace(salesRepName) &&
                    string.IsNullOrWhiteSpace(customerCode) &&
                    string.IsNullOrWhiteSpace(customerName))
                {
                    continue;
                }

                totalRows++;

                if (string.IsNullOrWhiteSpace(salesRepCode) ||
                    string.IsNullOrWhiteSpace(salesRepName) ||
                    string.IsNullOrWhiteSpace(customerCode) ||
                    string.IsNullOrWhiteSpace(customerName))
                {
                    failedRows++;
                    continue;
                }

                var successfulVisitValueText = GetTableValue(table, row, headers, "قيمه الزياره الناجحه");

                visitRows.Add(new SalesAnalyticsDailyVisitImportDto
                {
                    ReportDate = reportDate.Date,

                    SupervisorName = GetTableValue(table, row, headers, "المشرف"),
                    CityName = GetTableValue(table, row, headers, "المدينة"),
                    VisitCode = GetTableValue(table, row, headers, "كود الزيارة"),

                    SalesRepCode = salesRepCode,
                    SalesRepName = salesRepName,

                    CustomerCode = customerCode,
                    CustomerName = customerName,

                    VisitStatus = GetTableValue(table, row, headers, "حالة الزيارة"),
                    NegativeReason = GetTableValue(table, row, headers, "سبب الزيارة السلبيه"),

                    SuccessfulVisitValue = ParseDecimal(successfulVisitValueText),

                    VisitStartTime = ParseDateTime(GetTableValue(table, row, headers, "بداية الزيارة")),
                    VisitEndTime = ParseDateTime(GetTableValue(table, row, headers, "نهاية الزيارة")),

                    VisitDurationText = GetTableValue(table, row, headers, "الفرق بين الزيارات")
                });
            }

            _repository.ReplaceDailyVisitsReport(visitRows, uploadBatchId, reportDate.Date);

            var status = failedRows > 0
                ? SalesAnalyticsUploadStatus.PartiallyImported
                : SalesAnalyticsUploadStatus.Success;

            _repository.CompleteUploadBatch(
                uploadBatchId,
                status,
                totalRows,
                visitRows.Count,
                failedRows);

            return new SalesAnalyticsImportResultDto
            {
                Success = true,
                TotalRows = totalRows,
                ImportedRows = visitRows.Count,
                FailedRows = failedRows,
                Message = $"Daily visits report imported successfully. Imported: {visitRows.Count}, Failed: {failedRows}"
            };
        }
        catch (Exception ex)
        {
            _repository.CompleteUploadBatch(
                uploadBatchId,
                SalesAnalyticsUploadStatus.Failed,
                0,
                0,
                0,
                ex.Message);

            return new SalesAnalyticsImportResultDto
            {
                Success = false,
                Message = ex.Message
            };
        }
    }

    #region Helper Methods
    private static int FindHeaderRow(IXLWorksheet worksheet)
    {
        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 0;
        var rowsToCheck = Math.Min(lastRow, 10);

        for (var row = 1; row <= rowsToCheck; row++)
        {
            var rowValues = worksheet.Row(row)
                .CellsUsed()
                .Select(x => x.GetString().Trim())
                .ToList();

            if (rowValues.Contains("Customer") &&
                rowValues.Contains("Name 1"))
            {
                return row;
            }
        }

        return 0;
    }

    private static Dictionary<string, int> BuildHeaderMap(IXLWorksheet worksheet, int headerRow)
    {
        var headers = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        var lastColumn = worksheet.Row(headerRow).LastCellUsed()?.Address.ColumnNumber ?? 0;

        for (var col = 1; col <= lastColumn; col++)
        {
            var header = worksheet.Cell(headerRow, col).GetString().Trim();

            if (string.IsNullOrWhiteSpace(header))
                continue;

            if (!headers.ContainsKey(header))
            {
                headers.Add(header, col);
            }
        }

        return headers;
    }

    private static string GetValue(IXLWorksheet worksheet,int row,Dictionary<string, int> headers,string headerName)
    {
        if (!headers.TryGetValue(headerName, out var column))
            return string.Empty;

        return worksheet.Cell(row, column).GetString().Trim();
    }

    private static DateTime? GetDateValue(IXLWorksheet worksheet,int row,Dictionary<string, int> headers,string headerName)
    {
        if (!headers.TryGetValue(headerName, out var column))
            return null;

        var cell = worksheet.Cell(row, column);

        if (cell.DataType == XLDataType.DateTime)
            return cell.GetDateTime();

        var value = cell.GetString().Trim();

        if (DateTime.TryParse(value, out var date))
            return date;

        return null;
    }

    private static int FindDailySalesHeaderRow(DataTable table)
    {
        var rowsToCheck = Math.Min(table.Rows.Count, 20);

        for (var row = 0; row < rowsToCheck; row++)
        {
            var values = table.Rows[row].ItemArray
                .Select(x => NormalizeHeader(x?.ToString()))
                .ToList();

            if (values.Contains(NormalizeHeader("الكود")) &&
                values.Contains(NormalizeHeader("الخط")) &&
                values.Contains(NormalizeHeader("الكمية")) &&
                values.Contains(NormalizeHeader("اسم الصنف")) &&
                values.Contains(NormalizeHeader("القيمة")))
            {
                return row;
            }
        }

        return -1;
    }

    private static Dictionary<string, int> BuildDataTableHeaderMap(DataTable table, int headerRowIndex)
    {
        var headers = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        for (var col = 0; col < table.Columns.Count; col++)
        {
            var header = NormalizeHeader(table.Rows[headerRowIndex][col]?.ToString());

            if (string.IsNullOrWhiteSpace(header))
                continue;

            if (!headers.ContainsKey(header))
            {
                headers.Add(header, col);
            }
        }

        return headers;
    }

    private static string GetTableValue(DataTable table,int row,Dictionary<string, int> headers,string headerName)
    {
        var normalizedHeader = NormalizeHeader(headerName);

        if (!headers.TryGetValue(normalizedHeader, out var column))
            return string.Empty;

        return table.Rows[row][column]?.ToString()?.Trim() ?? string.Empty;
    }

    private static string NormalizeHeader(string? value)
    {
        return value?
            .Trim()
            .Replace("\n", " ")
            .Replace("\r", " ")
            .Replace("  ", " ")
            ?? string.Empty;
    }

    private static decimal ParseDecimal(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return 0;

        value = value.Trim()
            .Replace(",", "");

        if (decimal.TryParse(value, out var result))
            return result;

        return 0;
    }

    private static int FindDailyVisitsHeaderRow(DataTable table)
    {
        var rowsToCheck = Math.Min(table.Rows.Count, 20);

        for (var row = 0; row < rowsToCheck; row++)
        {
            var values = table.Rows[row].ItemArray
                .Select(x => NormalizeHeader(x?.ToString()))
                .ToList();

            if (values.Contains(NormalizeHeader("كود المندوب")) &&
                values.Contains(NormalizeHeader("اسم المندوب")) &&
                values.Contains(NormalizeHeader("كود العميل")) &&
                values.Contains(NormalizeHeader("اسم العميل")) &&
                values.Contains(NormalizeHeader("حالة الزيارة")))
            {
                return row;
            }
        }

        return -1;
    }

    private static DateTime? ParseDateTime(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        value = value.Trim();

        if (DateTime.TryParse(value, out var result))
            return result;

        return null;
    }

    #endregion
}