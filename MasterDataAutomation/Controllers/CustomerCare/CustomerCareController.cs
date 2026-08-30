using System.Security.Claims;
using MasterDataAutomation.Application.Modules.CustomerCare.Dtos;
using MasterDataAutomation.Application.Modules.CustomerCare.Interfaces;
using MasterDataAutomation.Domain.Modules.CustomerCare.Enums;
using MasterDataAutomation.Web.ViewModels.CustomerCare;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MasterDataAutomation.Application.Common.Security;
using Microsoft.AspNetCore.Hosting;

namespace MasterDataAutomation.Web.Controllers.CustomerCare;


[Authorize(Roles = CustomerCareRoles.All)]
public class CustomerCareController : Controller
{
    private readonly ICustomerCareTicketService _ticketService;
    private readonly IWebHostEnvironment _environment;

    public CustomerCareController(
        ICustomerCareTicketService ticketService,
        IWebHostEnvironment environment)
    {
        _ticketService = ticketService;
        _environment = environment;
    }

    // =========================================================
    // Queue
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> Index(
        CustomerCareQueueViewModel model,
        CancellationToken cancellationToken)
    {
        var filter = new CustomerCareTicketFilterDto
        {
            Search = model.Search,
            Status = model.Status,
            Priority = model.Priority,
            Source = model.Source,
            CategoryId = model.CategoryId,
            OverdueOnly = model.OverdueOnly,
            FollowUpOnly = model.FollowUpOnly
        };

        if (User.IsInRole(CustomerCareRoles.Agent))
        {
            filter.CreatedByUserId =
                GetCurrentUserId();
        }

        model.Tickets =
            await _ticketService.GetQueueAsync(
                filter,
                cancellationToken);

        model.Categories =
            await _ticketService.GetCategoriesAsync(
                cancellationToken);

        return View(model);
    }

    // =========================================================
    // Create GET
    // =========================================================

    [HttpGet]
    [Authorize(Roles = CustomerCareRoles.All)]
    public async Task<IActionResult> Create(
        CancellationToken cancellationToken)
    {
        var model =
            new CustomerCareCreateViewModel();

        await LoadLookupsAsync(
            model,
            cancellationToken);

        return View(model);
    }

    // =========================================================
    // Create POST
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = CustomerCareRoles.All)]
    public async Task<IActionResult> Create(
        CustomerCareCreateViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await LoadLookupsAsync(
                model,
                cancellationToken);

            return View(model);
        }

        var userId =
            GetCurrentUserId();

        var userName =
            User.FindFirst("FullName")?.Value
            ?? User.Identity?.Name
            ?? "User";

        try
        {
            var categories =
                 await _ticketService.GetCategoriesAsync(
                       cancellationToken);

            var category =
                categories.FirstOrDefault(
                    x => x.Id == model.CategoryId);

            var isQuality =
                category?.Code == "QUALITY";

            CreateCustomerCareQualityDetailDto? qualityDetail = null;

            if (isQuality)
            {
                qualityDetail =
                    new CreateCustomerCareQualityDetailDto
                    {
                        ProductId = model.ProductId,

                        ProductName =
                            model.ProductName
                            ?? string.Empty,

                        BatchNumber =
                            model.BatchNumber,

                        ProductionDate =
                            model.ProductionDate,

                        ExpiryDate =
                            model.ExpiryDate,

                        QualityIssueType =
                            model.QualityIssueType,

                        QualityIssueDetails =
                            model.QualityIssueDetails,

                        SampleRequired =
                            model.SampleRequired,

                        HasHealthRisk =
                            model.HasHealthRisk,

                        HasLegalRisk =
                            model.HasLegalRisk,

                        RiskNotes =
                            model.RiskNotes
                    };
            }

            var dto =
                new CreateCustomerCareTicketDto
                {
                    CustomerId =
                        model.CustomerId,

                    CustomerName =
                        model.CustomerName,

                    CustomerPhone =
                        model.CustomerPhone,

                    CustomerAddress =
                        model.CustomerAddress,

                    Source =
                        model.Source,

                    Type =
                        model.Type,

                    CategoryId =
                        model.CategoryId,

                    SubCategoryId =
                        model.SubCategoryId,

                    Description =
                        model.Description,

                    Priority =
                        model.Priority,

                    AssignedDepartmentId =
                        model.AssignedDepartmentId,

                    NextFollowUpAt =
                        model.NextFollowUpAt,

                    QualityDetail =
                        qualityDetail
                };

            var ticket =
                await _ticketService.CreateAsync(
                    dto,
                    userId,
                    userName,
                    cancellationToken);

            // =====================================================
            // Optional quality image upload during ticket creation
            // =====================================================

            if (isQuality &&
                model.QualityImage != null &&
                model.QualityImage.Length > 0)
            {
                const long maxFileSize =
                    5 * 1024 * 1024;

                var allowedExtensions =
                    new[]
                    {
                        ".jpg",
                        ".jpeg",
                        ".png",
                        ".webp"
                    };

                var extension =
                    Path.GetExtension(
                            model.QualityImage.FileName)
                        .ToLowerInvariant();

                if (model.QualityImage.Length > maxFileSize)
                {
                    TempData["Error"] =
                        $"تم إنشاء التذكرة {ticket.TicketNumber}، لكن لم يتم رفع الصورة لأن حجمها أكبر من 5 ميجابايت.";

                    return RedirectToAction(
                        nameof(Details),
                        new { id = ticket.Id });
                }

                if (!allowedExtensions.Contains(extension))
                {
                    TempData["Error"] =
                        $"تم إنشاء التذكرة {ticket.TicketNumber}، لكن نوع الصورة غير مسموح.";

                    return RedirectToAction(
                        nameof(Details),
                        new { id = ticket.Id });
                }

                var originalFileName =
                    Path.GetFileName(
                        model.QualityImage.FileName);

                var storedFileName =
                    $"{Guid.NewGuid():N}{extension}";

                var relativeFolder =
                    Path.Combine(
                        "uploads",
                        "customer-care",
                        "quality",
                        ticket.Id.ToString());

                var physicalFolder =
                    Path.Combine(
                        _environment.WebRootPath,
                        relativeFolder);

                Directory.CreateDirectory(
                    physicalFolder);

                var physicalPath =
                    Path.Combine(
                        physicalFolder,
                        storedFileName);

                await using (
                    var stream =
                        new FileStream(
                            physicalPath,
                            FileMode.Create))
                {
                    await model.QualityImage.CopyToAsync(
                        stream,
                        cancellationToken);
                }

                var publicPath =
                    "/" +
                    Path.Combine(
                            relativeFolder,
                            storedFileName)
                        .Replace("\\", "/");

                try
                {
                    await _ticketService.AddAttachmentAsync(
                        ticket.Id,
                        new AddCustomerCareAttachmentDto
                        {
                            FileName =
                                originalFileName,

                            StoredFileName =
                                storedFileName,

                            FilePath =
                                publicPath,

                            ContentType =
                                model.QualityImage.ContentType,

                            FileSize =
                                model.QualityImage.Length,

                            AttachmentType =
                                "QualityEvidence",

                            Description =
                                model.QualityImageDescription
                        },
                        userId,
                        userName,
                        cancellationToken);
                }
                catch
                {
                    if (System.IO.File.Exists(
                            physicalPath))
                    {
                        System.IO.File.Delete(
                            physicalPath);
                    }

                    throw;
                }
            }

            TempData["Success"] =
                $"تم إنشاء التذكرة {ticket.TicketNumber} بنجاح.";

            return RedirectToAction(
                nameof(Details),
                new { id = ticket.Id });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(
                string.Empty,
                ex.Message);

            await LoadLookupsAsync(
                model,
                cancellationToken);

            return View(model);
        }
    }

    // =========================================================
    // Sub Categories AJAX
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> SubCategories(
        int categoryId,
        CancellationToken cancellationToken)
    {
        var items =
            await _ticketService.GetSubCategoriesAsync(
                categoryId,
                cancellationToken);

        return Json(items);
    }

    // =========================================================
    // Duplicate Check
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> CheckCustomer(
        string phone,
        CancellationToken cancellationToken)
    {
        var result =
            await _ticketService.CheckDuplicateCustomerAsync(
                phone,
                cancellationToken);

        return Json(result);
    }



    [HttpGet]
    public async Task<IActionResult> Details(
    int id,
    CancellationToken cancellationToken)
    {
        var ticket =
            await _ticketService.GetDetailsAsync(
                id,
                cancellationToken);

        if (ticket == null)
            return NotFound();

        if (User.IsInRole(CustomerCareRoles.Agent) &&
    ticket.CreatedByUserId != GetCurrentUserId())
        {
            return Forbid();
        }

        ViewBag.Departments =
            await _ticketService.GetDepartmentsAsync(
                cancellationToken);

        return View(ticket);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = CustomerCareRoles.SupervisorsAndAbove)]
    public async Task<IActionResult> Assign(
    int id,
    AssignCustomerCareTicketDto dto,
    CancellationToken cancellationToken)
    {
        await _ticketService.AssignAsync(
            id,
            dto,
            GetCurrentUserId(),
            GetCurrentUserName(),
            cancellationToken);

        return RedirectToAction(
            nameof(Details),
            new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = CustomerCareRoles.SupervisorsAndAbove)]
    public async Task<IActionResult> ChangeStatus(
    int id,
    ChangeCustomerCareTicketStatusDto dto,
    CancellationToken cancellationToken)
    {
        await _ticketService.ChangeStatusAsync(
            id,
            dto,
            GetCurrentUserId(),
            GetCurrentUserName(),
            cancellationToken);

        return RedirectToAction(
            nameof(Details),
            new { id });
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = CustomerCareRoles.All)]
    public async Task<IActionResult> AddAction(
    int id,
    AddCustomerCareTicketActionDto dto,
    CancellationToken cancellationToken)
    {
        await _ticketService.AddActionAsync(
            id,
            dto,
            GetCurrentUserId(),
            GetCurrentUserName(),
            cancellationToken);

        return RedirectToAction(
            nameof(Details),
            new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = CustomerCareRoles.SupervisorsAndAbove)]
    public async Task<IActionResult> Resolve(
    int id,
    ResolveCustomerCareTicketDto dto,
    CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.ResolutionSummary))
        {
            TempData["Error"] =
                "لا يمكن إغلاق الحالة كـ Resolved بدون كتابة ملخص الحل.";

            return RedirectToAction(
                nameof(Details),
                new { id });
        }

        try
        {
            await _ticketService.ResolveAsync(
                id,
                dto,
                GetCurrentUserId(),
                GetCurrentUserName(),
                cancellationToken);

            TempData["Success"] =
                "تم حل الـTicket بنجاح.";
        }
        catch (Exception ex)
        {
            TempData["Error"] =
                ex.Message;
        }

        return RedirectToAction(
            nameof(Details),
            new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = CustomerCareRoles.ManagersAndAdmin)]
    public async Task<IActionResult> Reopen(
    int id,
    string reason,
    CancellationToken cancellationToken)
    {
        await _ticketService.ReopenAsync(
            id,
            reason,
            GetCurrentUserId(),
            GetCurrentUserName(),
            cancellationToken);

        return RedirectToAction(
            nameof(Details),
            new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = CustomerCareRoles.ManagersAndAdmin)]
    public async Task<IActionResult> Close(
    int id,
    string reason,
    CancellationToken cancellationToken)
    {
        try
        {
            await _ticketService.CloseAsync(
                id,
                reason,
                GetCurrentUserId(),
                GetCurrentUserName(),
                cancellationToken);

            TempData["Success"] =
                "تم إغلاق الـTicket بنجاح.";
        }
        catch (Exception ex)
        {
            TempData["Error"] =
                ex.Message;
        }

        return RedirectToAction(
            nameof(Details),
            new { id });
    }


    [HttpGet]
    [Authorize(Roles = CustomerCareRoles.SupervisorsAndAbove)]
    public async Task<IActionResult> Dashboard(
    DateTime? fromDate,
    DateTime? toDate,
    CancellationToken cancellationToken)
    {
        var model =
            await _ticketService.GetDashboardAsync(
                fromDate,
                toDate,
                cancellationToken);

        ViewBag.FromDate = fromDate;

        ViewBag.ToDate = toDate;

        return View(model);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = CustomerCareRoles.SupervisorsAndAbove)]
    public async Task<IActionResult> AddGift(
    int id,
    AddCustomerCareTicketGiftDto dto,
    CancellationToken cancellationToken)
    {
        try
        {
            await _ticketService.AddGiftAsync(
                id,
                dto,
                GetCurrentUserId(),
                GetCurrentUserName(),
                cancellationToken);

            TempData["Success"] =
                "تم تسجيل الهدية بنجاح.";
        }
        catch (Exception ex)
        {
            TempData["Error"] =
                ex.Message;
        }

        return RedirectToAction(
            nameof(Details),
            new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = CustomerCareRoles.ManagersAndAdmin)]
    public async Task<IActionResult> DeleteGift(
    int id,
    int giftId,
    CancellationToken cancellationToken)
    {
        try
        {
            await _ticketService.DeleteGiftAsync(
                id,
                giftId,
                cancellationToken);

            TempData["Success"] =
                "تم حذف الهدية.";
        }
        catch (Exception ex)
        {
            TempData["Error"] =
                ex.Message;
        }

        return RedirectToAction(
            nameof(Details),
            new { id });
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = CustomerCareRoles.All)]
    public async Task<IActionResult> UploadQualityImage(
    int id,
    UploadQualityImageViewModel model,
    CancellationToken cancellationToken)
    {
        if (model.Image == null ||
            model.Image.Length == 0)
        {
            TempData["Error"] =
                "اختر صورة لرفعها.";

            return RedirectToAction(
                nameof(Details),
                new { id });
        }

        const long maxFileSize =
            5 * 1024 * 1024;

        if (model.Image.Length > maxFileSize)
        {
            TempData["Error"] =
                "حجم الصورة يجب ألا يتجاوز 5 ميجابايت.";

            return RedirectToAction(
                nameof(Details),
                new { id });
        }

        var allowedExtensions =
            new[]
            {
            ".jpg",
            ".jpeg",
            ".png",
            ".webp"
            };

        var extension =
            Path.GetExtension(
                    model.Image.FileName)
                .ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
        {
            TempData["Error"] =
                "نوع الملف غير مسموح. المسموح: JPG, JPEG, PNG, WEBP.";

            return RedirectToAction(
                nameof(Details),
                new { id });
        }

        var originalFileName =
            Path.GetFileName(
                model.Image.FileName);

        var storedFileName =
            $"{Guid.NewGuid():N}{extension}";

        var relativeFolder =
            Path.Combine(
                "uploads",
                "customer-care",
                "quality",
                id.ToString());

        var physicalFolder =
            Path.Combine(
                _environment.WebRootPath,
                relativeFolder);

        Directory.CreateDirectory(
            physicalFolder);

        var physicalPath =
            Path.Combine(
                physicalFolder,
                storedFileName);

        await using (
            var stream =
                new FileStream(
                    physicalPath,
                    FileMode.Create))
        {
            await model.Image.CopyToAsync(
                stream,
                cancellationToken);
        }

        var publicPath =
            "/" +
            Path.Combine(
                    relativeFolder,
                    storedFileName)
                .Replace("\\", "/");

        try
        {
            await _ticketService.AddAttachmentAsync(
                id,
                new AddCustomerCareAttachmentDto
                {
                    FileName =
                        originalFileName,

                    StoredFileName =
                        storedFileName,

                    FilePath =
                        publicPath,

                    ContentType =
                        model.Image.ContentType,

                    FileSize =
                        model.Image.Length,

                    AttachmentType =
                        "QualityEvidence",

                    Description =
                        model.Description
                },
                GetCurrentUserId(),
                GetCurrentUserName(),
                cancellationToken);

            TempData["Success"] =
                "تم رفع صورة شكوى الجودة بنجاح.";
        }
        catch (Exception ex)
        {
            if (System.IO.File.Exists(
                    physicalPath))
            {
                System.IO.File.Delete(
                    physicalPath);
            }

            TempData["Error"] =
                ex.Message;
        }

        return RedirectToAction(
            nameof(Details),
            new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = CustomerCareRoles.ManagersAndAdmin)]
    public async Task<IActionResult> DeleteQualityImage(
    int id,
    int attachmentId,
    CancellationToken cancellationToken)
    {
        try
        {
            var relativePath =
                await _ticketService.DeleteAttachmentAsync(
                    id,
                    attachmentId,
                    cancellationToken);

            if (!string.IsNullOrWhiteSpace(
                    relativePath))
            {
                var safeRelativePath =
                    relativePath
                        .TrimStart('/')
                        .Replace(
                            "/",
                            Path.DirectorySeparatorChar.ToString());

                var physicalPath =
                    Path.Combine(
                        _environment.WebRootPath,
                        safeRelativePath);

                if (System.IO.File.Exists(
                        physicalPath))
                {
                    System.IO.File.Delete(
                        physicalPath);
                }
            }

            TempData["Success"] =
                "تم حذف الصورة.";
        }
        catch (Exception ex)
        {
            TempData["Error"] =
                ex.Message;
        }

        return RedirectToAction(
            nameof(Details),
            new { id });
    }




    // =========================================================
    // Helpers
    // =========================================================
    private string GetCurrentUserName()
    {
        return User.FindFirst("FullName")?.Value
            ?? User.Identity?.Name
            ?? "User";
    }
    private async Task LoadLookupsAsync(
        CustomerCareCreateViewModel model,
        CancellationToken cancellationToken)
    {
        model.Categories =
            await _ticketService.GetCategoriesAsync(
                cancellationToken);

        model.Departments =
            await _ticketService.GetDepartmentsAsync(
                cancellationToken);

        if (model.CategoryId.HasValue)
        {
            model.SubCategories =
                await _ticketService.GetSubCategoriesAsync(
                    model.CategoryId.Value,
                    cancellationToken);
        }
    }

    private int GetCurrentUserId()
    {
        var claim =
            User.FindFirstValue(
                ClaimTypes.NameIdentifier);

        if (!int.TryParse(
            claim,
            out var userId))
        {
            throw new InvalidOperationException(
                "Current user could not be identified.");
        }

        return userId;
    }
}