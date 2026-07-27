using MasterDataAutomation.Application.Modules.ProductMaster.Dtos;
using MasterDataAutomation.Application.Modules.ProductMaster.Interfaces;
using MasterDataAutomation.Web.ViewModels.ProductMaster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasterDataAutomation.Web.Controllers.ProductMaster;

[Authorize(Roles = "Admin,ExecutiveManager")]
public class ProductExecutiveManagerController : Controller
{
    private readonly IProductRequestRepository _productRequestRepository;

    public ProductExecutiveManagerController(IProductRequestRepository productRequestRepository)
    {
        _productRequestRepository = productRequestRepository;
    }

    public IActionResult Index()
    {
        var requests = _productRequestRepository.GetSubmittedToExecutiveManager();

        var model = new ExecutiveProductDashboardViewModel
        {
            PendingPricingCount = requests.Count,
            RecentPricingRequests = requests
                .Take(5)
                .ToList()
        };

        return View(model);
    }
    public IActionResult Submitted()
    {
        var requests = _productRequestRepository.GetSubmittedToExecutiveManager();

        return View(requests);
    }

    [HttpGet]
    public IActionResult SetPrices(int id)
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
    public IActionResult SetPrices(ProductRequestDto model)
    {
        if (model.Id <= 0)
        {
            TempData["Error"] = "Invalid product request.";
            return RedirectToAction(nameof(Submitted));
        }

        if (!model.OutletSellingPrice.HasValue || model.OutletSellingPrice <= 0)
        {
            ModelState.AddModelError(nameof(model.OutletSellingPrice), "Outlet Price is required.");
        }

        if (!model.RetailSellingPrice.HasValue || model.RetailSellingPrice <= 0)
        {
            ModelState.AddModelError(nameof(model.RetailSellingPrice), "Retail price is required.");
        }

        if (!ModelState.IsValid)
        {
            var request = _productRequestRepository.GetById(model.Id);

            if (request == null)
            {
                TempData["Error"] = "Product request not found.";
                return RedirectToAction(nameof(Submitted));
            }

            request.OutletSellingPrice = model.OutletSellingPrice;
            request.RetailSellingPrice = model.RetailSellingPrice;

            return View(request);
        }

        var created = _productRequestRepository.FinalApproveByExecutiveManager(
            model.Id,
            model.OutletSellingPrice.Value,
            model.RetailSellingPrice.Value);

        if (!created)
        {
            TempData["Error"] = "Product could not be created. It may already exist in products master.";
            return RedirectToAction(nameof(Submitted));
        }

        TempData["Success"] = "Product created successfully in products master.";

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

        _productRequestRepository.RejectByExecutiveManager(id, rejectionReason);

        TempData["Success"] = "Product request rejected successfully.";

        return RedirectToAction(nameof(Submitted));
    }
}