using ClosedXML.Excel;
using MasterDataAutomation.Application.Modules.SalesAnalytics.Dtos;
using MasterDataAutomation.Application.Modules.SalesAnalytics.Enums;
using MasterDataAutomation.Application.Modules.SalesAnalytics.Interfaces;

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

    private static string GetValue(
        IXLWorksheet worksheet,
        int row,
        Dictionary<string, int> headers,
        string headerName)
    {
        if (!headers.TryGetValue(headerName, out var column))
            return string.Empty;

        return worksheet.Cell(row, column).GetString().Trim();
    }

    private static DateTime? GetDateValue(
        IXLWorksheet worksheet,
        int row,
        Dictionary<string, int> headers,
        string headerName)
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
    #endregion
}