using ClosedXML.Excel;
using MasterDataAutomation.Application.Interfaces.Repositories;
using MasterDataAutomation.Application.Interfaces.Services;
using MasterDataAutomation.Application.Modules.CustomerModification.Interfaces;
using MasterDataAutomation.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasterDataAutomation.Web.Controllers;

[Authorize(Roles = "Manager,Admin")]
public class ManagerDashboardController : Controller
{
    private readonly ICustomerDraftRepository _customerDraftRepository;
    private readonly ISettingsService _settingsService;
    private readonly IHistoryService _historyService;
    private readonly ICustomerImportService _customerImportService;
    private readonly ICustomerModificationRequestRepository _customerModificationRequestRepository;

    public ManagerDashboardController(
        ICustomerDraftRepository customerDraftRepository,
        ISettingsService settingsService,
        IHistoryService historyService,
        ICustomerImportService customerImportService,
            ICustomerModificationRequestRepository customerModificationRequestRepository)
    {
        _customerDraftRepository = customerDraftRepository;
        _settingsService = settingsService;
        _historyService = historyService;
        _customerImportService = customerImportService;
        _customerModificationRequestRepository = customerModificationRequestRepository;

    }

    [HttpGet]
    public IActionResult Index()
    {
        var draftCustomers = _customerDraftRepository.GetAll();
        var history = _historyService.GetAll();

        var pendingModificationsCount = _customerModificationRequestRepository.GetSubmitted().Count;
        var approvedModificationsCount = _customerModificationRequestRepository.GetApproved().Count;

        ViewBag.PendingModificationsCount = pendingModificationsCount;
        ViewBag.ApprovedModificationsCount = approvedModificationsCount;

        var model = new ManagerDashboardViewModel
        {
            DraftCustomersCount = draftCustomers.Count,
            LastBpCode = _settingsService.GetLastBpCode(),
            HistoryCount = history.Count,
            BranchSummary = _customerDraftRepository.GetBranchSummary(),
            RecentHistory = history.OrderByDescending(x => x.Date).Take(5).ToList(),
            SubmittedCount = _customerDraftRepository.GetSubmittedCount(),
            ApprovedCount = _customerDraftRepository.GetApprovedCount(),
            RejectedCount = _customerDraftRepository.GetRejectedCount(),
            PendingSubmittedCustomers = _customerDraftRepository.GetSubmittedForReview().Take(5).ToList(),
        };

        return View(model);
    }

    [HttpGet]
    public IActionResult ExportHistoryReport()
    {
        var history = _historyService.GetAll()
            .OrderByDescending(x => x.Date)
            .ToList();

        using var workbook = new XLWorkbook();

        var worksheet = workbook.Worksheets.Add("History");

        worksheet.Cell(1, 1).Value = "Date";
        worksheet.Cell(1, 2).Value = "File Name";
        worksheet.Cell(1, 3).Value = "Customers Count";
        worksheet.Cell(1, 4).Value = "First BP Code";
        worksheet.Cell(1, 5).Value = "Last BP Code";
        worksheet.Cell(1, 6).Value = "Status";

        var headerRange = worksheet.Range(1, 1, 1, 6);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

        for (int i = 0; i < history.Count; i++)
        {
            var row = i + 2;

            worksheet.Cell(row, 1).Value = history[i].Date.ToString("yyyy-MM-dd HH:mm");
            worksheet.Cell(row, 2).Value = history[i].FileName;
            worksheet.Cell(row, 3).Value = history[i].CustomersCount;
            worksheet.Cell(row, 4).Value = history[i].FirstBpCode;
            worksheet.Cell(row, 5).Value = history[i].LastBpCode;
            worksheet.Cell(row, 6).Value = history[i].Status;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();

        workbook.SaveAs(stream);

        var fileName = $"History_Report_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

        return File(
            stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }

    [HttpGet]
    public IActionResult SubmittedCustomers()
    {
        var customers = _customerDraftRepository.GetSubmittedForReview();

        return View(customers);
    }

    [HttpPost]
    public IActionResult ApproveCustomer(int id)
    {
        _customerDraftRepository.ApproveById(id);

        TempData["Success"] = "Customer approved successfully.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult ApproveSubmitted()
    {
        var customers = _customerDraftRepository.GetSubmitted();

        if (!customers.Any())
        {
            TempData["Error"] = "No submitted customers to approve.";
            return RedirectToAction(nameof(SubmittedCustomers));
        }

        _customerDraftRepository.ApproveSubmitted();

        TempData["Success"] = "Submitted customers approved successfully.";

        return RedirectToAction(nameof(SubmittedCustomers));
    }

    [HttpPost]
    public IActionResult RejectSubmitted()
    {
        var customers = _customerDraftRepository.GetSubmitted();

        if (!customers.Any())
        {
            TempData["Error"] = "No submitted customers to reject.";
            return RedirectToAction(nameof(SubmittedCustomers));
        }

        _customerDraftRepository.RejectSubmitted();

        TempData["Success"] = "Submitted customers rejected.";

        return RedirectToAction(nameof(SubmittedCustomers));
    }

    [HttpGet]
    public IActionResult RejectedCustomers()
    {
        var customers = _customerDraftRepository.GetRejected();

        return View(customers);
    }

    [HttpPost]
    public IActionResult RejectCustomer(int id, string rejectionReason)
    {
        if (string.IsNullOrWhiteSpace(rejectionReason))
        {
            TempData["Error"] = "Rejection reason is required.";
            return RedirectToAction(nameof(Index));
        }

        _customerDraftRepository.RejectById(id, rejectionReason);

        TempData["Success"] = "Customer rejected successfully.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> GenerateApproved()
    {
        var result = await _customerImportService.GenerateFromApprovedAsync();

        if (result.Errors.Any())
        {
            TempData["Error"] = result.Errors.First().Message;
            return RedirectToAction(nameof(ApprovedCustomers));
        }

        return File(result.File,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            result.FileName);
    }

    [HttpGet]
    public IActionResult ApprovedCustomers()
    {
        var customers = _customerDraftRepository.GetApprovedForReview();

        return View(customers);
    }

    [HttpPost]
    public IActionResult DeleteApprovedCustomer(int id)
    {
        _customerDraftRepository.DeleteApprovedById(id);

        TempData["Success"] = "Approved customer deleted successfully.";

        return RedirectToAction(nameof(ApprovedCustomers));
    }

    [HttpPost]
    public IActionResult ApproveAllSubmitted()
    {
        var submittedCount = _customerDraftRepository.GetSubmittedCount();

        if (submittedCount == 0)
        {
            TempData["Error"] = "No submitted customers to approve.";
            return RedirectToAction(nameof(Index));
        }

        _customerDraftRepository.ApproveSubmitted();

        TempData["Success"] = $"{submittedCount} submitted customers approved successfully.";

        return RedirectToAction(nameof(Index));
    }
}