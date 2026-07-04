using MasterDataAutomation.Application.Interfaces.Services;
using MasterDataAutomation.Application.Modules.ProductMaster.Dtos;
using MasterDataAutomation.Application.Modules.ProductMaster.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClosedXML.Excel;
using System.Globalization;

namespace MasterDataAutomation.Web.Controllers.ProductMaster;

[Authorize]
public class ProductsController : Controller
{
    private readonly IProductRepository _productRepository;
    private readonly IActivityLogService _activityLogService;

    public ProductsController(IProductRepository productRepository,
            IActivityLogService activityLogService)

    {
        _productRepository = productRepository;
        _activityLogService = activityLogService;

    }

    
    public IActionResult Index(string? searchTerm)
    {
        var products = _productRepository.Search(searchTerm);
        ViewBag.SearchTerm = searchTerm;

        return View(products);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,ExecutiveManager")]
    public IActionResult Create()
    {
        return View(new ProductDto
        {
            Currency = "EGP",
            IsActive = true
        });
    }

    [HttpPost]
    [Authorize(Roles = "Admin,ExecutiveManager")]
    public IActionResult Create(ProductDto model)
    {
        if (string.IsNullOrWhiteSpace(model.ItemCode))
        {
            ModelState.AddModelError(nameof(model.ItemCode), "Item code is required.");
        }

        if (string.IsNullOrWhiteSpace(model.ItemName))
        {
            ModelState.AddModelError(nameof(model.ItemName), "Item name is required.");
        }

        if (!string.IsNullOrWhiteSpace(model.ItemCode)
            && _productRepository.GetByItemCode(model.ItemCode.Trim()) != null)
        {
            ModelState.AddModelError(nameof(model.ItemCode), "Item code already exists.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        _productRepository.Add(model);

        TempData["Success"] = "Product added successfully.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Roles = "Admin,ExecutiveManager")]
    public IActionResult Edit(int id)
    {
        var product = _productRepository.GetById(id);

        if (product == null)
        {
            TempData["Error"] = "Product not found.";
            return RedirectToAction(nameof(Index));
        }

        return View(product);
    }

    [Authorize(Roles = "Admin,ExecutiveManager")]
    [HttpPost]
    public IActionResult Edit(ProductDto model)
    {
        if (model.Id <= 0)
        {
            TempData["Error"] = "Invalid product.";
            return RedirectToAction(nameof(Index));
        }

        if (string.IsNullOrWhiteSpace(model.ItemName))
        {
            ModelState.AddModelError(nameof(model.ItemName), "Item name is required.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        _productRepository.Update(model);

        TempData["Success"] = "Product updated successfully.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,ExecutiveManager")]
    public IActionResult Delete(int id)
    {
        var product = _productRepository.GetById(id);

        if (product == null)
        {
            TempData["Error"] = "Product not found.";
            return RedirectToAction(nameof(Index));
        }

        _productRepository.Delete(id);

        _activityLogService.Log(
        action: "Delete",
        module: "Products",
        entityName: "Product",
        entityId: id,
        description: $"Deleted product {product.ItemCode} - {product.ItemName}");


        TempData["Success"] = "Product deleted successfully.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Roles = "Admin,ExecutiveManager")]
    public IActionResult Import()
    {
        return View();
    }

    [HttpGet]
    [Authorize(Roles = "Admin,ExecutiveManager")]
    public IActionResult DownloadImportTemplate()
    {
        using var workbook = new XLWorkbook();

        var worksheet = workbook.Worksheets.Add("Products");

        worksheet.Cell(1, 1).Value = "ItemCode";
        worksheet.Cell(1, 2).Value = "ItemName";
        worksheet.Cell(1, 3).Value = "RetailSellingPrice";
        worksheet.Cell(1, 4).Value = "OutletSellingPrice";
        worksheet.Cell(1, 5).Value = "Currency";
        worksheet.Cell(1, 6).Value = "IsActive";

        worksheet.Range(1, 1, 1, 6).Style.Font.Bold = true;
        worksheet.Range(1, 1, 1, 6).Style.Fill.BackgroundColor = XLColor.LightBlue;

        worksheet.Cell(2, 1).Value = "P001";
        worksheet.Cell(2, 2).Value = "Vanilla Cup 100ml";
        worksheet.Cell(2, 3).Value = 15;
        worksheet.Cell(2, 4).Value = 12;
        worksheet.Cell(2, 5).Value = "EGP";
        worksheet.Cell(2, 6).Value = "TRUE";

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        return File(
            stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "Products_Import_Template.xlsx");
    }

    [HttpPost]
    [Authorize(Roles = "Admin,ExecutiveManager")]
    public IActionResult Import(IFormFile file)
    {
        var errors = new List<string>();

        if (file == null || file.Length == 0)
        {
            ViewBag.Errors = new List<string> { "Please select an Excel file." };
            return View();
        }

        if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
        {
            ViewBag.Errors = new List<string> { "Only .xlsx files are allowed." };
            return View();
        }

        var insertedCount = 0;
        var updatedCount = 0;

        using var stream = file.OpenReadStream();
        using var workbook = new XLWorkbook(stream);

        var worksheet = workbook.Worksheets.FirstOrDefault();

        if (worksheet == null)
        {
            ViewBag.Errors = new List<string> { "Excel file does not contain any worksheets." };
            return View();
        }

        var itemCodeCol = FindColumn(worksheet, "ItemCode");
        var itemNameCol = FindColumn(worksheet, "ItemName");
        var retailPriceCol = FindColumn(worksheet, "RetailSellingPrice");
        var outletPriceCol = FindColumn(worksheet, "OutletSellingPrice");
        var currencyCol = FindColumn(worksheet, "Currency");
        var isActiveCol = FindColumn(worksheet, "IsActive");

        if (itemCodeCol == 0) errors.Add("Missing column: ItemCode");
        if (itemNameCol == 0) errors.Add("Missing column: ItemName");
        if (retailPriceCol == 0) errors.Add("Missing column: RetailSellingPrice");
        if (outletPriceCol == 0) errors.Add("Missing column: OutletSellingPrice");

        if (errors.Any())
        {
            ViewBag.Errors = errors;
            return View();
        }

        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;
        var excelItemCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (int row = 2; row <= lastRow; row++)
        {
            var itemCode = worksheet.Cell(row, itemCodeCol).GetString().Trim();
            var itemName = worksheet.Cell(row, itemNameCol).GetString().Trim();

            if (string.IsNullOrWhiteSpace(itemCode) && string.IsNullOrWhiteSpace(itemName))
                continue;

            if (string.IsNullOrWhiteSpace(itemCode))
            {
                errors.Add($"Row {row}: ItemCode is required.");
                continue;
            }

            if (string.IsNullOrWhiteSpace(itemName))
            {
                errors.Add($"Row {row}: ItemName is required.");
                continue;
            }

            if (!excelItemCodes.Add(itemCode))
            {
                errors.Add($"Row {row}: Duplicate ItemCode in Excel file: {itemCode}");
                continue;
            }

            if (!TryGetDecimal(worksheet.Cell(row, retailPriceCol), out var retailSellingPrice))
            {
                errors.Add($"Row {row}: RetailSellingPrice is invalid.");
                continue;
            }

            if (!TryGetDecimal(worksheet.Cell(row, outletPriceCol), out var outletSellingPrice))
            {
                errors.Add($"Row {row}: OutletSellingPrice is invalid.");
                continue;
            }

            var currency = currencyCol > 0
                ? worksheet.Cell(row, currencyCol).GetString().Trim()
                : "EGP";

            if (string.IsNullOrWhiteSpace(currency))
                currency = "EGP";

            var isActive = true;

            if (isActiveCol > 0)
            {
                isActive = ReadIsActive(worksheet.Cell(row, isActiveCol).GetString());
            }

            var existingProduct = _productRepository.GetByItemCode(itemCode);

            if (existingProduct == null)
            {
                _productRepository.Add(new ProductDto
                {
                    ItemCode = itemCode,
                    ItemName = itemName,
                    RetailSellingPrice = retailSellingPrice,
                    OutletSellingPrice = outletSellingPrice,
                    Currency = currency,
                    IsActive = isActive
                });

                insertedCount++;
            }
            else
            {
                existingProduct.ItemName = itemName;
                existingProduct.RetailSellingPrice = retailSellingPrice;
                existingProduct.OutletSellingPrice = outletSellingPrice;
                existingProduct.Currency = currency;
                existingProduct.IsActive = isActive;

                _productRepository.Update(existingProduct);

                updatedCount++;
            }
        }

        ViewBag.SuccessMessage = $"Import completed. Inserted: {insertedCount}, Updated: {updatedCount}.";
        ViewBag.Errors = errors;

        return View();
    }

    private static int FindColumn(IXLWorksheet worksheet, string columnName)
    {
        var lastColumn = worksheet.Row(1).LastCellUsed()?.Address.ColumnNumber ?? 0;

        for (int col = 1; col <= lastColumn; col++)
        {
            var header = worksheet.Cell(1, col).GetString().Trim();

            if (header.Equals(columnName, StringComparison.OrdinalIgnoreCase))
                return col;
        }

        return 0;
    }

    private static bool TryGetDecimal(IXLCell cell, out decimal value)
    {
        value = 0;

        if (cell.TryGetValue(out decimal numericValue))
        {
            value = numericValue;
            return true;
        }

        var text = cell.GetString().Trim();

        return decimal.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out value)
            || decimal.TryParse(text, NumberStyles.Any, CultureInfo.CurrentCulture, out value);
    }

    private static bool ReadIsActive(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return true;

        value = value.Trim().ToLower();

        return value == "true"
            || value == "1"
            || value == "yes"
            || value == "active";
    }

    [HttpGet]
    public IActionResult Export(string? searchTerm)
    {
        var products = string.IsNullOrWhiteSpace(searchTerm)
            ? _productRepository.GetAll()
            : _productRepository.Search(searchTerm);

        using var workbook = new XLWorkbook();

        var worksheet = workbook.Worksheets.Add("Products");

        worksheet.Cell(1, 1).Value = "ItemCode";
        worksheet.Cell(1, 2).Value = "ItemName";
        worksheet.Cell(1, 3).Value = "RetailSellingPrice";
        worksheet.Cell(1, 4).Value = "OutletSellingPrice";
        worksheet.Cell(1, 5).Value = "Currency";
        worksheet.Cell(1, 6).Value = "IsActive";
        worksheet.Cell(1, 7).Value = "RetailValidFrom";
        worksheet.Cell(1, 8).Value = "RetailValidTo";
        worksheet.Cell(1, 9).Value = "OutletValidFrom";
        worksheet.Cell(1, 10).Value = "OutletValidTo";

        var headerRange = worksheet.Range(1, 1, 1, 10);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        var row = 2;

        foreach (var product in products)
        {
            worksheet.Cell(row, 1).Value = product.ItemCode;
            worksheet.Cell(row, 2).Value = product.ItemName;
            worksheet.Cell(row, 3).Value = product.RetailSellingPrice;
            worksheet.Cell(row, 4).Value = product.OutletSellingPrice;
            worksheet.Cell(row, 5).Value = product.Currency ?? "EGP";
            worksheet.Cell(row, 6).Value = product.IsActive ? "Active" : "Inactive";

            if (product.RetailValidFrom.HasValue)
                worksheet.Cell(row, 7).Value = product.RetailValidFrom.Value;

            if (product.RetailValidTo.HasValue)
                worksheet.Cell(row, 8).Value = product.RetailValidTo.Value;

            if (product.MarketValidFrom.HasValue)
                worksheet.Cell(row, 9).Value = product.MarketValidFrom.Value;

            if (product.MarketValidTo.HasValue)
                worksheet.Cell(row, 10).Value = product.MarketValidTo.Value;

            row++;
        }

        worksheet.Column(3).Style.NumberFormat.Format = "#,##0.00";
        worksheet.Column(4).Style.NumberFormat.Format = "#,##0.00";

        worksheet.Column(7).Style.DateFormat.Format = "dd/MM/yyyy";
        worksheet.Column(8).Style.DateFormat.Format = "dd/MM/yyyy";
        worksheet.Column(9).Style.DateFormat.Format = "dd/MM/yyyy";
        worksheet.Column(10).Style.DateFormat.Format = "dd/MM/yyyy";



        worksheet.SheetView.FreezeRows(1);
        worksheet.RangeUsed()?.SetAutoFilter();

        worksheet.Column(1).Style.NumberFormat.Format = "@";
        worksheet.Column(1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

        worksheet.Column(2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

        worksheet.Columns().AdjustToContents();

        worksheet.Column(1).Width = 18;
        worksheet.Column(2).Width = 35;
        worksheet.Column(3).Width = 18;
        worksheet.Column(4).Width = 18;


        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        var fileName = $"Products_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

        return File(
            stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }
}