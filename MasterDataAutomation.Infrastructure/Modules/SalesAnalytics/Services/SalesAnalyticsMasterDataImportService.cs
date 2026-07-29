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
    private readonly ISalesDistrictMonthlyTargetRepository _targetRepository;

    public SalesAnalyticsMasterDataImportService(ISalesAnalyticsMasterDataRepository repository, ISalesDistrictMonthlyTargetRepository targetRepository )
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

    public SalesAnalyticsImportResultDto ImportMtdSalesReport(
    Stream fileStream,
    string originalFileName,
    DateTime toDate,
    string? uploadedBy)
    {
        var uploadBatchId = _repository.CreateUploadBatch(
            SalesAnalyticsUploadFileType.MtdSalesReport,
            originalFileName,
            toDate,
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
                    "No sheets found in the uploaded MTD sales report.");

                return new SalesAnalyticsImportResultDto
                {
                    Success = false,
                    Message = "No sheets found in the uploaded MTD sales report.",
                    TotalRows = 0,
                    ImportedRows = 0,
                    FailedRows = 0
                };
            }

            var salesRows = new List<SalesAnalyticsMtdSalesImportDto>();
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

                    salesRows.Add(new SalesAnalyticsMtdSalesImportDto
                    {
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

            _repository.ReplaceMtdSalesReport(salesRows, uploadBatchId, toDate);

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
                    ? "MTD sales report imported successfully."
                    : "MTD sales report imported with some skipped rows.",
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

    private static string NormalizeCode(string? code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return string.Empty;

        return code.Trim().TrimStart('0');
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

    private static bool IsSameText(string? value, string expected)
    {
        return NormalizeHeader(value) == NormalizeHeader(expected);
    }

    #endregion
}