using MasterDataAutomation.Application.Modules.ProductMaster.Dtos;
using MasterDataAutomation.Application.Modules.ProductMaster.Interfaces;
using MasterDataAutomation.Web.ViewModels.ProductMaster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasterDataAutomation.Web.Controllers.ProductMaster;

[Authorize(Roles = "Admin,AccountsManager")]
public class ProductAccountsManagerController : Controller
{
    private readonly IProductRequestRepository _productRequestRepository;

    public ProductAccountsManagerController(IProductRequestRepository productRequestRepository)
    {
        _productRequestRepository = productRequestRepository;
    }

    public IActionResult Index()
    {
        var requests = _productRequestRepository.GetSubmittedToAccountsManager();

        var model = new AccountsManagerProductDashboardViewModel
        {
            PendingReviewCount = requests.Count,
            RecentPendingRequests = requests
                .Take(5)
                .ToList()
        };

        return View(model);
    }

    public IActionResult Submitted()
    {
        var requests = _productRequestRepository.GetSubmittedToAccountsManager();

        return View(requests);
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var request = _productRequestRepository.GetById(id);

        if (request == null)
        {
            TempData["Error"] = "Product request not found.";
            return RedirectToAction(nameof(Submitted));
        }

        return View(request);
    }

    [HttpPost]
    public IActionResult Edit(ProductRequestDto model)
    {
        if (model.Id <= 0)
        {
            TempData["Error"] = "Invalid product request.";
            return RedirectToAction(nameof(Submitted));
        }

        if (string.IsNullOrWhiteSpace(model.ItemName))
        {
            ModelState.AddModelError(nameof(model.ItemName), "Item name is required.");
        }

        if (model.PurchasePrice <= 0)
        {
            ModelState.AddModelError(nameof(model.PurchasePrice), "Purchase price must be greater than zero.");
        }

        if (model.PiecesCount <= 0)
        {
            ModelState.AddModelError(nameof(model.PiecesCount), "Pieces count must be greater than zero.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        _productRequestRepository.UpdateAccountsData(model);

        TempData["Success"] = "Product request updated successfully.";

        return RedirectToAction(nameof(Submitted));
    }

    [HttpPost]
    public IActionResult Approve(int id)
    {
        _productRequestRepository.ApproveByAccountsManager(id);

        TempData["Success"] = "Product request approved and sent to executive manager.";

        return RedirectToAction(nameof(Submitted));
    }

    [HttpPost]
    public IActionResult Reject(int id, string rejectionReason)
    {
        if (string.IsNullOrWhiteSpace(rejectionReason))
        {
            TempData["Error"] = "Rejection reason is required.";
            return RedirectToAction(nameof(Submitted));
        }

        _productRequestRepository.RejectByAccountsManager(id, rejectionReason);

        TempData["Success"] = "Product request rejected successfully.";

        return RedirectToAction(nameof(Submitted));
    }
}