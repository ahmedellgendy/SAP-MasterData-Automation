using MasterDataAutomation.Application.Modules.ProductMaster.Dtos;
using MasterDataAutomation.Application.Modules.ProductMaster.Interfaces;
using MasterDataAutomation.Web.ViewModels.ProductMaster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasterDataAutomation.Web.Controllers.ProductMaster;

[Authorize(Roles = "Admin,AccountsUser")]
public class ProductRequestsController : Controller
{
    private readonly IProductRequestRepository _productRequestRepository;
    private readonly IProductRepository _productRepository;

    public ProductRequestsController(
        IProductRequestRepository productRequestRepository,
        IProductRepository productRepository)
    {
        _productRequestRepository = productRequestRepository;
        _productRepository = productRepository;
    }

    public IActionResult Index()
    {
        var drafts = _productRequestRepository.GetDrafts();
        var submitted = _productRequestRepository.GetSubmittedToAccountsManager();

        var model = new AccountsUserProductDashboardViewModel
        {
            DraftCount = drafts.Count,
            SubmittedCount = submitted.Count,

            RecentDrafts = drafts
                .Take(5)
                .ToList(),

            RecentSubmitted = submitted
                .Take(5)
                .ToList()
        };

        return View(model);
    }
    public IActionResult Drafts()
    {
        var requests = _productRequestRepository.GetDrafts();

        return View(requests);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateProductRequestDto
        {
            Currency = "EGP"
        });
    }

    [HttpPost]
    public IActionResult Create(CreateProductRequestDto model)
    {
        if (string.IsNullOrWhiteSpace(model.ItemCode))
        {
            ModelState.AddModelError(nameof(model.ItemCode), "Item code is required.");
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


        if (_productRepository.GetByItemCode(model.ItemCode.Trim()) != null)
        {
            ModelState.AddModelError(nameof(model.ItemCode), "This item code already exists in products master.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        _productRequestRepository.Create(model);

        TempData["Success"] = "Product request created successfully.";

        return RedirectToAction(nameof(Drafts));
    }

    [HttpPost]
    public IActionResult Submit(int id)
    {
        _productRequestRepository.SubmitDraft(id);

        TempData["Success"] = "Product request submitted to accounts manager.";

        return RedirectToAction(nameof(Drafts));
    }
}