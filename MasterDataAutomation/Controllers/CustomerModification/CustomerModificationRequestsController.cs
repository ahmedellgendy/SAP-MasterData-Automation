using MasterDataAutomation.Application.Modules.CustomerModification.Dtos;
using MasterDataAutomation.Application.Modules.CustomerModification.Enums;
using MasterDataAutomation.Application.Modules.CustomerModification.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MasterDataAutomation.Web.Controllers.CustomerModification;

[Authorize(Roles = "Admin,Sales")]
public class CustomerModificationRequestsController : Controller
{
    private readonly ICustomerModificationRequestRepository _repository;

    public CustomerModificationRequestsController(ICustomerModificationRequestRepository repository)
    {
        _repository = repository;
    }

    public IActionResult Index()
    {
        return RedirectToAction(nameof(Drafts));
    }

    [HttpGet]
    public IActionResult Create()
    {
        LoadBranches();

        return View(new CreateCustomerModificationRequestDto());
    }

    [HttpPost]
    public IActionResult Create(CreateCustomerModificationRequestDto model)
    {
        ValidateCreateModel(model);

        if (!ModelState.IsValid)
        {
            LoadBranches();
            return View(model);
        }

        model.BranchName = GetBranchName(model.BranchId!.Value);

        var createdBy = User.Identity?.Name ?? "Unknown";

        _repository.Create(model, createdBy);

        TempData["Success"] = "تم حفظ طلب تعديل العميل كمسودة.";

        return RedirectToAction(nameof(Drafts));
    }

    [HttpGet]
    public IActionResult Drafts()
    {
        var requests = _repository.GetDrafts();

        return View(requests);
    }

    [HttpPost]
    public IActionResult Submit(int id)
    {
        _repository.SubmitDraft(id);

        TempData["Success"] = "تم إرسال طلب تعديل العميل للمراجعة.";

        return RedirectToAction(nameof(Drafts));
    }

    [HttpGet]
    public IActionResult Rejected()
    {
        var requests = _repository.GetRejected();

        return View(requests);
    }

    [HttpGet]
    public IActionResult EditRejected(int id)
    {
        var request = _repository.GetById(id);

        if (request == null)
        {
            TempData["Error"] = "لم يتم العثور على الطلب.";
            return RedirectToAction(nameof(Rejected));
        }

        if (request.Status != CustomerModificationStatus.Rejected)
        {
            TempData["Error"] = "يمكن تعديل الطلبات المرفوضة فقط.";
            return RedirectToAction(nameof(Rejected));
        }

        LoadBranches();

        var model = new CreateCustomerModificationRequestDto
        {
            BranchId = request.BranchId,
            BranchName = request.BranchName,
            MarketCode = request.MarketCode,
            MarketName = request.MarketName,
            CurrentCustomerType = request.CurrentCustomerType,
            ModificationType = request.ModificationType,
            NewMarketName = request.NewMarketName,
            Notes = request.Notes
        };

        ViewBag.RequestId = id;
        ViewBag.RejectionReason = request.RejectionReason;

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EditRejected(int id, CreateCustomerModificationRequestDto model)
    {
        ValidateCreateModel(model);

        if (!ModelState.IsValid)
        {
            LoadBranches();

            ViewBag.RequestId = id;

            var oldRequest = _repository.GetById(id);
            ViewBag.RejectionReason = oldRequest?.RejectionReason;

            return View(model);
        }

        model.BranchName = GetBranchName(model.BranchId!.Value);

        var result = _repository.UpdateRejectedAndResubmit(id, model);

        if (!result)
        {
            TempData["Error"] = "لم يتم تعديل الطلب. تأكد أن الطلب مازال مرفوض.";
            return RedirectToAction(nameof(Rejected));
        }

        TempData["Success"] = "تم تعديل الطلب وإرساله للمراجعة مرة أخرى.";

        return RedirectToAction(nameof(Rejected));
    }


    private void ValidateCreateModel(CreateCustomerModificationRequestDto model)
    {
        if (!model.BranchId.HasValue || model.BranchId <= 0)
        {
            ModelState.AddModelError(nameof(model.BranchId), "اختر الفرع.");
        }

        if (string.IsNullOrWhiteSpace(model.MarketCode))
        {
            ModelState.AddModelError(nameof(model.MarketCode), "كود الماركت مطلوب.");
        }

        if (string.IsNullOrWhiteSpace(model.MarketName))
        {
            ModelState.AddModelError(nameof(model.MarketName), "اسم الماركت مطلوب.");
        }

        if (!model.CurrentCustomerType.HasValue)
        {
            ModelState.AddModelError(nameof(model.CurrentCustomerType), "اختر نوع العميل الحالي.");
        }

        if (!model.ModificationType.HasValue)
        {
            ModelState.AddModelError(nameof(model.ModificationType), "اختر نوع التعديل.");
        }

        if (model.ModificationType == CustomerModificationType.ChangeName &&
            string.IsNullOrWhiteSpace(model.NewMarketName))
        {
            ModelState.AddModelError(nameof(model.NewMarketName), "اسم الماركت الجديد مطلوب عند اختيار تعديل اسم.");
        }
    }

    private void LoadBranches()
    {
        ViewBag.Branches = new List<SelectListItem>
        {
            new SelectListItem { Value = "1", Text = "العاشر" },
            new SelectListItem { Value = "2", Text = "الجيزة" },
            new SelectListItem { Value = "3", Text = "إسكندرية" },
            new SelectListItem { Value = "4", Text = "المنصورة" }
        };
    }

    private static string GetBranchName(int branchId)
    {
        return branchId switch
        {
            1 => "العاشر",
            2 => "الجيزة",
            3 => "إسكندرية",
            4 => "المنصورة",
            _ => "غير محدد"
        };
    }
}