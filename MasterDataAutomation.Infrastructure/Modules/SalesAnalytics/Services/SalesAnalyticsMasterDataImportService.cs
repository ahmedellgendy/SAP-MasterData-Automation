using ClosedXML.Excel;
using ExcelDataReader;
using MasterDataAutomation.Application.Modules.SalesAnalytics.Dtos;
using MasterDataAutomation.Application.Modules.SalesAnalytics.Enums;
using MasterDataAutomation.Application.Modules.SalesAnalytics.Interfaces;
using System.Data;
using System.Globalization;
using System.Text;

namespace MasterDataAutomation.Infrastructure.Modules.SalesAnalytics.Services;

public class SalesAnalyticsMasterDataImportService : ISalesAnalyticsMasterDataImportService
{
    private readonly ISalesAnalyticsMasterDataRepository _repository;
    private readonly ISalesDistrictMonthlyTargetRepository _targetRepository;

    public SalesAnalyticsMasterDataImportService(
        ISalesAnalyticsMasterDataRepository repository,
        ISalesDistrictMonthlyTargetRepository targetRepository)
    {
        _repository = repository;
        _targetRepository = targetRepository;
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
                    CustomerCode = NormalizeCode(customerCode),
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
                .GroupBy(x => NormalizeCode(x.CustomerCode))
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
                    SalesRepCode = NormalizeCode(salesRepCode),
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
                .GroupBy(x => NormalizeCode(x.SalesRepCode))
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
        var uploadBatchId = _repository.CreateUploadBatch(
            SalesAnalyticsUploadFileType.DailySalesReport,
            originalFileName,
            reportDate.Date,
            uploadedBy);

        try
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            using var reader = ExcelReaderFactory.CreateReader(fileStream);
            var dataSet = reader.AsDataSet();

            if (dataSet.Tables.Count == 0)
            {
                _repository.CompleteUploadBatch(
                    uploadBatchId,
                    SalesAnalyticsUploadStatus.Failed,
                    0,
                    0,
                    0,
                    "No sheets found in the uploaded daily sales report.");

                return new SalesAnalyticsImportResultDto
                {
                    Success = false,
                    Message = "No sheets found in the uploaded daily sales report.",
                    TotalRows = 0,
                    ImportedRows = 0,
                    FailedRows = 0
                };
            }

            var salesRows = new List<SalesAnalyticsDailySalesImportDto>();
            var failedRows = 0;

            string? currentCityCode = null;
            string? currentCityName = null;
            string? currentCustomerCode = null;
            string? currentCustomerName = null;

            foreach (DataTable table in dataSet.Tables)
            {
                for (var rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++)
                {
                    var groupType = GetCellText(table, rowIndex, 44);

                    if (IsSameText(groupType, "المدينة"))
                    {
                        currentCityCode = NormalizeCode(GetCellText(table, rowIndex, 14));
                        currentCityName = GetCellText(table, rowIndex, 28).Trim();
                        continue;
                    }

                    if (IsSameText(groupType, "العميل"))
                    {
                        currentCustomerCode = NormalizeCode(GetCellText(table, rowIndex, 14));
                        currentCustomerName = GetCellText(table, rowIndex, 28).Trim();
                        continue;
                    }

                    var productCode = GetCellText(table, rowIndex, 48);
                    var productName = GetFirstAvailableCellText(table, rowIndex, 40, 39);
                    var unit = GetCellText(table, rowIndex, 37);

                    if (string.IsNullOrWhiteSpace(productCode) ||
                        string.IsNullOrWhiteSpace(productName))
                    {
                        continue;
                    }

                    var quantity = ParseDecimal(GetCellText(table, rowIndex, 31));
                    var salesAmount = ParseDecimal(GetCellText(table, rowIndex, 25));
                    var discountAmount = ParseDecimal(GetCellText(table, rowIndex, 21));
                    var taxPercentage = ParseDecimal(GetCellText(table, rowIndex, 4));
                    var taxAmount = ParseDecimal(GetCellText(table, rowIndex, 16));
                    var totalBeforeTax = ParseDecimal(GetCellText(table, rowIndex, 10));
                    var totalAfterTax = ParseDecimal(GetCellText(table, rowIndex, 0));

                    if (string.IsNullOrWhiteSpace(currentCustomerCode) ||
                        string.IsNullOrWhiteSpace(currentCustomerName))
                    {
                        failedRows++;
                        continue;
                    }

                    if (salesAmount == 0 && totalAfterTax == 0 && quantity == 0)
                    {
                        continue;
                    }

                    salesRows.Add(new SalesAnalyticsDailySalesImportDto
                    {
                        LineCode = currentCityCode ?? string.Empty,
                        LineName = currentCityName ?? string.Empty,

                        CityCode = currentCityCode,
                        CityName = currentCityName,

                        CustomerCode = currentCustomerCode,
                        CustomerName = currentCustomerName,

                        ProductCode = NormalizeCode(productCode),
                        ProductName = productName.Trim(),
                        Unit = unit.Trim(),

                        Quantity = quantity,
                        SalesAmount = salesAmount,
                        DiscountAmount = discountAmount,
                        TaxPercentage = taxPercentage,
                        TaxAmount = taxAmount,
                        TotalBeforeTax = totalBeforeTax,
                        TotalAfterTax = totalAfterTax
                    });
                }
            }

            if (!salesRows.Any())
            {
                _repository.CompleteUploadBatch(
                    uploadBatchId,
                    SalesAnalyticsUploadStatus.Failed,
                    0,
                    0,
                    failedRows,
                    "Could not detect any daily sales rows in this Crystal Report.");

                return new SalesAnalyticsImportResultDto
                {
                    Success = false,
                    Message = "Could not detect any daily sales rows. تأكد إن التقرير هو تفاصيل تحليل المبيعات - العميل / المدينة - القيمة.",
                    TotalRows = 0,
                    ImportedRows = 0,
                    FailedRows = failedRows
                };
            }

            _repository.ReplaceDailySalesReport(salesRows, uploadBatchId, reportDate.Date);

            var totalRows = salesRows.Count + failedRows;

            _repository.CompleteUploadBatch(
                uploadBatchId,
                failedRows > 0
                    ? SalesAnalyticsUploadStatus.PartiallyImported
                    : SalesAnalyticsUploadStatus.Success,
                totalRows,
                salesRows.Count,
                failedRows);

            return new SalesAnalyticsImportResultDto
            {
                Success = failedRows == 0,
                Message = failedRows == 0
                    ? "Daily sales report imported successfully."
                    : "Daily sales report imported with some skipped rows.",
                TotalRows = totalRows,
                ImportedRows = salesRows.Count,
                FailedRows = failedRows
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
                Message = ex.Message,
                TotalRows = 0,
                ImportedRows = 0,
                FailedRows = 0
            };
        }
    }

    public SalesAnalyticsImportResultDto ImportMtdSalesReport(
    Stream fileStream,
    string originalFileName,
    DateTime fromDate,
    DateTime toDate,
    string? uploadedBy)
    {
        var uploadBatchId = _repository.CreateUploadBatch(
            SalesAnalyticsUploadFileType.MtdSalesReport,
            originalFileName,
            toDate.Date,
            uploadedBy);

        try
        {
            Encoding.RegisterProvider(
                CodePagesEncodingProvider.Instance);

            using var reader =
                ExcelReaderFactory.CreateReader(fileStream);

            var salesRows =
                new List<SalesAnalyticsMtdSalesImportDto>();

            var failedRows = 0;

            string? currentCityCode = null;
            string? currentCityName = null;

            string? currentCustomerCode = null;
            string? currentCustomerName = null;

            var hasAnySheet = false;

            do
            {
                hasAnySheet = true;

                while (reader.Read())
                {
                    var groupType =
                        GetReaderCellText(
                            reader,
                            44);

                    if (IsSameText(
                        groupType,
                        "المدينة"))
                    {
                        currentCityCode =
                            NormalizeCode(
                                GetReaderCellText(
                                    reader,
                                    14));

                        currentCityName =
                            GetReaderCellText(
                                reader,
                                28)
                            .Trim();

                        continue;
                    }

                    if (IsSameText(
                        groupType,
                        "العميل"))
                    {
                        currentCustomerCode =
                            NormalizeCode(
                                GetReaderCellText(
                                    reader,
                                    14));

                        currentCustomerName =
                            GetReaderCellText(
                                reader,
                                28)
                            .Trim();

                        continue;
                    }

                    var productCode =
                        GetReaderCellText(
                            reader,
                            48);

                    var productName =
                        GetFirstAvailableReaderCellText(
                            reader,
                            40,
                            39);

                    var unit =
                        GetReaderCellText(
                            reader,
                            37);

                    if (
                        string.IsNullOrWhiteSpace(
                            productCode) ||
                        string.IsNullOrWhiteSpace(
                            productName))
                    {
                        continue;
                    }

                    var quantity =
                        ParseDecimal(
                            GetReaderCellText(
                                reader,
                                31));

                    var salesAmount =
                        ParseDecimal(
                            GetReaderCellText(
                                reader,
                                25));

                    var discountAmount =
                        ParseDecimal(
                            GetReaderCellText(
                                reader,
                                21));

                    var taxPercentage =
                        ParseDecimal(
                            GetReaderCellText(
                                reader,
                                4));

                    var taxAmount =
                        ParseDecimal(
                            GetReaderCellText(
                                reader,
                                16));

                    var totalBeforeTax =
                        ParseDecimal(
                            GetReaderCellText(
                                reader,
                                10));

                    var totalAfterTax =
                        ParseDecimal(
                            GetReaderCellText(
                                reader,
                                0));

                    if (
                        string.IsNullOrWhiteSpace(
                            currentCustomerCode) ||
                        string.IsNullOrWhiteSpace(
                            currentCustomerName))
                    {
                        failedRows++;
                        continue;
                    }

                    if (
                        salesAmount == 0 &&
                        totalAfterTax == 0 &&
                        quantity == 0)
                    {
                        continue;
                    }

                    salesRows.Add(
                        new SalesAnalyticsMtdSalesImportDto
                        {
                            CityCode =
                                currentCityCode,

                            CityName =
                                currentCityName,

                            CustomerCode =
                                currentCustomerCode,

                            CustomerName =
                                currentCustomerName,

                            ProductCode =
                                NormalizeCode(
                                    productCode),

                            ProductName =
                                productName.Trim(),

                            Unit =
                                unit.Trim(),

                            Quantity =
                                quantity,

                            SalesAmount =
                                salesAmount,

                            DiscountAmount =
                                discountAmount,

                            TaxPercentage =
                                taxPercentage,

                            TaxAmount =
                                taxAmount,

                            TotalBeforeTax =
                                totalBeforeTax,

                            TotalAfterTax =
                                totalAfterTax
                        });
                }
            }
            while (reader.NextResult());

            if (!hasAnySheet)
            {
                _repository.CompleteUploadBatch(
                    uploadBatchId,
                    SalesAnalyticsUploadStatus.Failed,
                    0,
                    0,
                    0,
                    "No sheets found in the uploaded Sales Range report.");

                return new SalesAnalyticsImportResultDto
                {
                    Success = false,
                    Message =
                        "No sheets found in the uploaded Sales Range report.",
                    TotalRows = 0,
                    ImportedRows = 0,
                    FailedRows = 0
                };
            }

            if (!salesRows.Any())
            {
                _repository.CompleteUploadBatch(
                    uploadBatchId,
                    SalesAnalyticsUploadStatus.Failed,
                    0,
                    0,
                    failedRows,
                    "Could not detect any sales rows in this Crystal Report.");

                return new SalesAnalyticsImportResultDto
                {
                    Success = false,

                    Message =
                        "Could not detect any sales rows. تأكد إن التقرير هو تفاصيل تحليل المبيعات - العميل / المدينة - القيمة.",

                    TotalRows = 0,
                    ImportedRows = 0,
                    FailedRows = failedRows
                };
            }

            _repository.ReplaceMtdSalesReport(
                salesRows,
                uploadBatchId,
                fromDate.Date,
                toDate.Date);

            var totalRows =
                salesRows.Count +
                failedRows;

            var status =
                failedRows > 0
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

                Message =
                    failedRows == 0
                        ? "Sales Range report imported successfully."
                        : "Sales Range report imported with some skipped rows.",

                TotalRows =
                    totalRows,

                ImportedRows =
                    salesRows.Count,

                FailedRows =
                    failedRows
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
                Message = ex.Message,
                TotalRows = 0,
                ImportedRows = 0,
                FailedRows = 0
            };
        }
    }

    public SalesAnalyticsImportResultDto ImportDailyVisitsReport(
        Stream fileStream,
        string originalFileName,
        DateTime reportDate,
        string? uploadedBy)
    {
        var uploadBatchId = _repository.CreateUploadBatch(
            SalesAnalyticsUploadFileType.DailyVisitsReport,
            originalFileName,
            reportDate.Date,
            uploadedBy);

        try
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            using var reader = ExcelReaderFactory.CreateReader(fileStream);
            var dataSet = reader.AsDataSet();

            if (dataSet.Tables.Count == 0)
            {
                _repository.CompleteUploadBatch(
                    uploadBatchId,
                    SalesAnalyticsUploadStatus.Failed,
                    0,
                    0,
                    0,
                    "No sheets found in the uploaded daily visits report.");

                return new SalesAnalyticsImportResultDto
                {
                    Success = false,
                    Message = "No sheets found in the uploaded daily visits report.",
                    TotalRows = 0,
                    ImportedRows = 0,
                    FailedRows = 0
                };
            }

            var visitRows = new List<SalesAnalyticsDailyVisitImportDto>();
            var failedRows = 0;

            string? currentSalesRepCode = null;
            string? currentSalesRepName = null;

            foreach (DataTable table in dataSet.Tables)
            {
                for (var rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++)
                {
                    if (ContainsNormalizedText(GetCellText(table, rowIndex, 49), "كود المندوب"))
                    {
                        var repName = GetCellText(table, rowIndex, 55);

                        if (!string.IsNullOrWhiteSpace(repName))
                            currentSalesRepName = repName.Trim();

                        continue;
                    }

                    if (ContainsNormalizedText(GetCellText(table, rowIndex, 23), "كود الموزع") ||
                        ContainsNormalizedText(GetCellText(table, rowIndex, 37), "الموزع"))
                    {
                        var repCode = GetCellText(table, rowIndex, 40);

                        if (!string.IsNullOrWhiteSpace(repCode))
                            currentSalesRepCode = NormalizeCode(repCode);

                        continue;
                    }

                    var detailMarker = GetCellText(table, rowIndex, 1);

                    if (!ContainsNormalizedText(detailMarker, "عرض"))
                        continue;

                    var successfulVisitValue = ParseDecimal(GetCellText(table, rowIndex, 7));
                    var negativeReason = GetCellText(table, rowIndex, 10);
                    var visitStatus = GetCellText(table, rowIndex, 20);
                    var durationText = GetCellText(table, rowIndex, 24);

                    var visitEndTime = ParseCrystalDateTime(GetCellText(table, rowIndex, 28));
                    var visitStartTime = ParseCrystalDateTime(GetCellText(table, rowIndex, 34));

                    var cityName = GetCellText(table, rowIndex, 50);
                    var customerName = GetCellText(table, rowIndex, 54);
                    var customerCode = GetCellText(table, rowIndex, 60);
                    var visitCode = GetCellText(table, rowIndex, 64);

                    if (string.IsNullOrWhiteSpace(customerCode) &&
                        string.IsNullOrWhiteSpace(customerName) &&
                        string.IsNullOrWhiteSpace(visitCode))
                    {
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(currentSalesRepCode) ||
                        string.IsNullOrWhiteSpace(currentSalesRepName) ||
                        string.IsNullOrWhiteSpace(customerCode) ||
                        string.IsNullOrWhiteSpace(customerName))
                    {
                        failedRows++;
                        continue;
                    }

                    visitRows.Add(new SalesAnalyticsDailyVisitImportDto
                    {
                        ReportDate = reportDate.Date,

                        SupervisorName = null,
                        CityName = cityName,
                        VisitCode = visitCode,

                        SalesRepCode = currentSalesRepCode,
                        SalesRepName = currentSalesRepName,

                        CustomerCode = NormalizeCode(customerCode),
                        CustomerName = customerName.Trim(),

                        VisitStatus = visitStatus,
                        NegativeReason = negativeReason,
                        SuccessfulVisitValue = successfulVisitValue,

                        VisitStartTime = visitStartTime,
                        VisitEndTime = visitEndTime,
                        VisitDurationText = durationText
                    });
                }
            }

            if (!visitRows.Any())
            {
                _repository.CompleteUploadBatch(
                    uploadBatchId,
                    SalesAnalyticsUploadStatus.Failed,
                    0,
                    0,
                    failedRows,
                    "Could not detect any visit detail rows in this Crystal Report.");

                return new SalesAnalyticsImportResultDto
                {
                    Success = false,
                    Message = "Could not detect any visit detail rows. تأكد إن التقرير هو تفاصيل الزيارات من SalesBuzz.",
                    TotalRows = 0,
                    ImportedRows = 0,
                    FailedRows = failedRows
                };
            }

            _repository.ReplaceDailyVisitsReport(visitRows, uploadBatchId, reportDate.Date);

            var totalRows = visitRows.Count + failedRows;

            _repository.CompleteUploadBatch(
                uploadBatchId,
                failedRows > 0
                    ? SalesAnalyticsUploadStatus.PartiallyImported
                    : SalesAnalyticsUploadStatus.Success,
                totalRows,
                visitRows.Count,
                failedRows);

            return new SalesAnalyticsImportResultDto
            {
                Success = failedRows == 0,
                Message = failedRows == 0
                    ? "Daily visits report imported successfully."
                    : "Daily visits report imported with some skipped rows.",
                TotalRows = totalRows,
                ImportedRows = visitRows.Count,
                FailedRows = failedRows
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
                Message = ex.Message,
                TotalRows = 0,
                ImportedRows = 0,
                FailedRows = 0
            };
        }
    }

    public SalesAnalyticsImportResultDto ImportMonthlyTargets(
        Stream fileStream,
        string originalFileName,
        string? uploadedBy)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        var uploadBatchId = _repository.CreateUploadBatch(
            SalesAnalyticsUploadFileType.MonthlyTargets,
            originalFileName,
            reportDate: null,
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
                .FirstOrDefault();

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

            var headerRowIndex = FindMonthlyTargetsHeaderRow(table);

            if (headerRowIndex == -1)
            {
                _repository.CompleteUploadBatch(
                    uploadBatchId,
                    SalesAnalyticsUploadStatus.Failed,
                    0,
                    0,
                    0,
                    "Could not find required target headers.");

                return new SalesAnalyticsImportResultDto
                {
                    Success = false,
                    Message = "Could not find required headers: Year, Month, SalesDistrictCode, MonthlySalesTarget, PlannedVisits."
                };
            }

            var headers = BuildDataTableHeaderMap(table, headerRowIndex);
            var salesDistrictOptions = _targetRepository.GetSalesDistrictOptions();

            var totalRows = 0;
            var importedRows = 0;
            var failedRows = 0;

            for (var row = headerRowIndex + 1; row < table.Rows.Count; row++)
            {
                var yearText = GetTableValue(table, row, headers, "Year");
                var monthText = GetTableValue(table, row, headers, "Month");
                var salesDistrictCode = GetTableValue(table, row, headers, "SalesDistrictCode");
                var monthlySalesTargetText = GetTableValue(table, row, headers, "MonthlySalesTarget");
                var plannedVisitsText = GetTableValue(table, row, headers, "PlannedVisits");

                if (string.IsNullOrWhiteSpace(yearText) &&
                    string.IsNullOrWhiteSpace(monthText) &&
                    string.IsNullOrWhiteSpace(salesDistrictCode) &&
                    string.IsNullOrWhiteSpace(monthlySalesTargetText) &&
                    string.IsNullOrWhiteSpace(plannedVisitsText))
                {
                    continue;
                }

                totalRows++;

                var year = ParseInt(yearText);
                var month = ParseInt(monthText);
                var monthlySalesTarget = ParseDecimal(monthlySalesTargetText);
                var plannedVisits = ParseInt(plannedVisitsText);

                if (year < 2000 ||
                    year > 2100 ||
                    month < 1 ||
                    month > 12 ||
                    string.IsNullOrWhiteSpace(salesDistrictCode) ||
                    monthlySalesTarget < 0 ||
                    plannedVisits < 0)
                {
                    failedRows++;
                    continue;
                }

                var option = salesDistrictOptions
                    .FirstOrDefault(x =>
                        NormalizeCode(x.SalesDistrictCode) == NormalizeCode(salesDistrictCode));

                if (option == null)
                {
                    failedRows++;
                    continue;
                }

                var dto = new SaveSalesDistrictMonthlyTargetDto
                {
                    Year = year,
                    Month = month,

                    SalesDistrictCode = option.SalesDistrictCode,
                    SalesDistrictName = option.SalesDistrictName,

                    BranchCode = option.BranchCode,
                    BranchName = option.BranchName,

                    MonthlySalesTarget = monthlySalesTarget,
                    PlannedVisits = plannedVisits
                };

                _targetRepository.SaveTarget(dto, uploadedBy);

                importedRows++;
            }

            var status = failedRows > 0
                ? SalesAnalyticsUploadStatus.PartiallyImported
                : SalesAnalyticsUploadStatus.Success;

            _repository.CompleteUploadBatch(
                uploadBatchId,
                status,
                totalRows,
                importedRows,
                failedRows);

            return new SalesAnalyticsImportResultDto
            {
                Success = true,
                TotalRows = totalRows,
                ImportedRows = importedRows,
                FailedRows = failedRows,
                Message = $"Monthly targets imported successfully. Imported: {importedRows}, Failed: {failedRows}"
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

    public SalesAnalyticsImportResultDto ImportMtdVisitsReport(
     Stream fileStream,
     string originalFileName,
     DateTime fromDate,
     DateTime toDate,
     string? uploadedBy)
    {
        var uploadBatchId = _repository.CreateUploadBatch(
            SalesAnalyticsUploadFileType.MtdVisitsReport,
            originalFileName,
            toDate.Date,
            uploadedBy);

        try
        {
            Encoding.RegisterProvider(
                CodePagesEncodingProvider.Instance);

            using var reader =
                ExcelReaderFactory.CreateReader(fileStream);

            var visitRows =
                new List<SalesAnalyticsMtdVisitImportDto>();

            var failedRows = 0;

            string? currentSalesRepCode = null;
            string? currentSalesRepName = null;

            var hasAnySheet = false;

            do
            {
                hasAnySheet = true;

                while (reader.Read())
                {
                    // =========================================
                    // Sales Rep Name
                    // =========================================

                    if (
                        ContainsNormalizedText(
                            GetReaderCellText(
                                reader,
                                49),
                            "كود المندوب"))
                    {
                        var repName =
                            GetReaderCellText(
                                reader,
                                55);

                        if (
                            !string.IsNullOrWhiteSpace(
                                repName))
                        {
                            currentSalesRepName =
                                repName.Trim();
                        }

                        continue;
                    }

                    // =========================================
                    // Sales Rep Code
                    // =========================================

                    if (
                        ContainsNormalizedText(
                            GetReaderCellText(
                                reader,
                                23),
                            "كود الموزع")
                        ||
                        ContainsNormalizedText(
                            GetReaderCellText(
                                reader,
                                37),
                            "الموزع"))
                    {
                        var repCode =
                            GetReaderCellText(
                                reader,
                                40);

                        if (
                            !string.IsNullOrWhiteSpace(
                                repCode))
                        {
                            currentSalesRepCode =
                                NormalizeCode(
                                    repCode);
                        }

                        continue;
                    }

                    // =========================================
                    // Visit Detail
                    // =========================================

                    var detailMarker =
                        GetReaderCellText(
                            reader,
                            1);

                    if (
                        !ContainsNormalizedText(
                            detailMarker,
                            "عرض"))
                    {
                        continue;
                    }

                    var successfulVisitValue =
                        ParseDecimal(
                            GetReaderCellText(
                                reader,
                                7));

                    var negativeReason =
                        GetReaderCellText(
                            reader,
                            10);

                    var visitStatus =
                        GetReaderCellText(
                            reader,
                            20);

                    var durationText =
                        GetReaderCellText(
                            reader,
                            24);

                    var visitEndTime =
                        ParseCrystalDateTime(
                            GetReaderCellText(
                                reader,
                                28));

                    var visitStartTime =
                        ParseCrystalDateTime(
                            GetReaderCellText(
                                reader,
                                34));

                    var cityName =
                        GetReaderCellText(
                            reader,
                            50);

                    var customerName =
                        GetReaderCellText(
                            reader,
                            54);

                    var customerCode =
                        GetReaderCellText(
                            reader,
                            60);

                    var visitCode =
                        GetReaderCellText(
                            reader,
                            64);

                    if (
                        string.IsNullOrWhiteSpace(
                            customerCode) &&
                        string.IsNullOrWhiteSpace(
                            customerName) &&
                        string.IsNullOrWhiteSpace(
                            visitCode))
                    {
                        continue;
                    }

                    if (
                        string.IsNullOrWhiteSpace(
                            currentSalesRepCode) ||
                        string.IsNullOrWhiteSpace(
                            currentSalesRepName) ||
                        string.IsNullOrWhiteSpace(
                            customerCode) ||
                        string.IsNullOrWhiteSpace(
                            customerName))
                    {
                        failedRows++;
                        continue;
                    }

                    visitRows.Add(
                        new SalesAnalyticsMtdVisitImportDto
                        {
                            VisitDate =
                                visitStartTime?.Date
                                ?? toDate.Date,

                            SupervisorName =
                                null,

                            CityName =
                                cityName,

                            VisitCode =
                                visitCode,

                            SalesRepCode =
                                currentSalesRepCode,

                            SalesRepName =
                                currentSalesRepName,

                            CustomerCode =
                                NormalizeCode(
                                    customerCode),

                            CustomerName =
                                customerName.Trim(),

                            VisitStatus =
                                visitStatus,

                            NegativeReason =
                                negativeReason,

                            SuccessfulVisitValue =
                                successfulVisitValue,

                            VisitStartTime =
                                visitStartTime,

                            VisitEndTime =
                                visitEndTime,

                            VisitDurationText =
                                durationText
                        });
                }
            }
            while (reader.NextResult());

            if (!hasAnySheet)
            {
                _repository.CompleteUploadBatch(
                    uploadBatchId,
                    SalesAnalyticsUploadStatus.Failed,
                    0,
                    0,
                    0,
                    "No sheets found in the uploaded Visits Range report.");

                return new SalesAnalyticsImportResultDto
                {
                    Success = false,
                    Message =
                        "No sheets found in the uploaded Visits Range report.",
                    TotalRows = 0,
                    ImportedRows = 0,
                    FailedRows = 0
                };
            }

            if (!visitRows.Any())
            {
                _repository.CompleteUploadBatch(
                    uploadBatchId,
                    SalesAnalyticsUploadStatus.Failed,
                    0,
                    0,
                    failedRows,
                    "Could not detect any visit detail rows in this Crystal Report.");

                return new SalesAnalyticsImportResultDto
                {
                    Success = false,

                    Message =
                        "Could not detect any visit detail rows. التقرير ده Crystal Report ومحتاج نفس فورمات تفاصيل الزيارات.",

                    TotalRows = 0,
                    ImportedRows = 0,
                    FailedRows = failedRows
                };
            }

            _repository.ReplaceMtdVisitsReport(
                visitRows,
                uploadBatchId,
                fromDate.Date,
                toDate.Date);

            var totalRows =
                visitRows.Count +
                failedRows;

            var status =
                failedRows > 0
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

                Message =
                    failedRows == 0
                        ? "Visits Range report imported successfully."
                        : "Visits Range report imported with some skipped rows.",

                TotalRows =
                    totalRows,

                ImportedRows =
                    visitRows.Count,

                FailedRows =
                    failedRows
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
                Message = ex.Message,
                TotalRows = 0,
                ImportedRows = 0,
                FailedRows = 0
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

    private static int FindMonthlyTargetsHeaderRow(DataTable table)
    {
        var rowsToCheck = Math.Min(table.Rows.Count, 20);

        for (var row = 0; row < rowsToCheck; row++)
        {
            var values = table.Rows[row].ItemArray
                .Select(x => NormalizeHeader(x?.ToString()))
                .ToList();

            if (values.Contains(NormalizeHeader("Year")) &&
                values.Contains(NormalizeHeader("Month")) &&
                values.Contains(NormalizeHeader("SalesDistrictCode")) &&
                values.Contains(NormalizeHeader("MonthlySalesTarget")) &&
                values.Contains(NormalizeHeader("PlannedVisits")))
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

    private static string GetTableValue(
        DataTable table,
        int row,
        Dictionary<string, int> headers,
        string headerName)
    {
        var normalizedHeader = NormalizeHeader(headerName);

        if (!headers.TryGetValue(normalizedHeader, out var column))
            return string.Empty;

        return table.Rows[row][column]?.ToString()?.Trim() ?? string.Empty;
    }

    private static string GetCellText(DataTable table, int rowIndex, int columnIndex)
    {
        if (rowIndex < 0 || rowIndex >= table.Rows.Count)
            return string.Empty;

        if (columnIndex < 0 || columnIndex >= table.Columns.Count)
            return string.Empty;

        return table.Rows[rowIndex][columnIndex]?.ToString()?.Trim() ?? string.Empty;
    }

    private static string GetFirstAvailableCellText(
        DataTable table,
        int rowIndex,
        params int[] columnIndexes)
    {
        foreach (var columnIndex in columnIndexes)
        {
            var value = GetCellText(table, rowIndex, columnIndex);

            if (!string.IsNullOrWhiteSpace(value))
                return value;
        }

        return string.Empty;
    }

    private static string GetFirstAvailableTableValue(
        DataTable table,
        int row,
        Dictionary<string, int> headers,
        params string[] headerNames)
    {
        foreach (var headerName in headerNames)
        {
            var value = GetTableValue(table, row, headers, headerName);

            if (!string.IsNullOrWhiteSpace(value))
                return value;
        }

        return string.Empty;
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

    private static string NormalizeCode(string? code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return string.Empty;

        return code.Trim().TrimStart('0');
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

    private static int ParseInt(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return 0;

        value = value.Trim()
            .Replace(",", "");

        if (int.TryParse(value, out var result))
            return result;

        return 0;
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

    private static bool IsSameText(string? value, string expected)
    {
        return NormalizeHeader(value) == NormalizeHeader(expected);
    }

    private static bool ContainsNormalizedText(string? value, string expected)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        return NormalizeHeader(value).Contains(NormalizeHeader(expected));
    }

    private static DateTime? ParseCrystalDateTime(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var cleanedValue = value.Trim();

        if (DateTime.TryParse(cleanedValue, out var dateTimeValue))
            return dateTimeValue;

        if (double.TryParse(
                cleanedValue,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out var oaDateValue))
        {
            try
            {
                return DateTime.FromOADate(oaDateValue);
            }
            catch
            {
                return null;
            }
        }

        return null;
    }



    private static string GetReaderCellText(
    IExcelDataReader reader,
    int columnIndex)
    {
        if (
            columnIndex < 0 ||
            columnIndex >= reader.FieldCount)
        {
            return string.Empty;
        }

        var value =
            reader.GetValue(columnIndex);

        if (
            value == null ||
            value == DBNull.Value)
        {
            return string.Empty;
        }

        return value
            .ToString()?
            .Trim()
            ?? string.Empty;
    }

    private static string GetFirstAvailableReaderCellText(
        IExcelDataReader reader,
        params int[] columnIndexes)
    {
        foreach (
            var columnIndex
            in columnIndexes)
        {
            var value =
                GetReaderCellText(
                    reader,
                    columnIndex);

            if (
                !string.IsNullOrWhiteSpace(
                    value))
            {
                return value;
            }
        }

        return string.Empty;
    }
    #endregion
}