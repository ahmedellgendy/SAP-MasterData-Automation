using MasterDataAutomation.Application.Modules.CustomerModification.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasterDataAutomation.Web.Controllers.CustomerModification;

[Authorize(Roles = "Admin,Manager")]
public class CustomerModificationManagerController : Controller
{
    private readonly ICustomerModificationRequestRepository _repository;

    public CustomerModificationManagerController(ICustomerModificationRequestRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var requests = _repository.GetSubmitted();

        return View(requests);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Approve(int id)
    {
        var result = _repository.Approve(id);

        if (!result)
        {
            TempData["Error"] = "لم يتم العثور على الطلب أو تم مراجعته بالفعل.";
            return RedirectToAction(nameof(Index));
        }

        TempData["Success"] = "تمت الموافقة على طلب تعديل العميل بنجاح.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Reject(int id, string rejectionReason)
    {
        if (string.IsNullOrWhiteSpace(rejectionReason))
        {
            TempData["Error"] = "سبب الرفض مطلوب.";
            return RedirectToAction(nameof(Index));
        }

        var result = _repository.Reject(id, rejectionReason);

        if (!result)
        {
            TempData["Error"] = "لم يتم العثور على الطلب أو تم مراجعته بالفعل.";
            return RedirectToAction(nameof(Index));
        }

        TempData["Success"] = "تم رفض طلب تعديل العميل.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Approved()
    {
        var requests = _repository.GetApproved();

        return View(requests);
    }
}