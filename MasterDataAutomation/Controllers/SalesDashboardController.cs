using MasterDataAutomation.Application.Interfaces.Repositories;
using MasterDataAutomation.Application.Interfaces.Services;
using MasterDataAutomation.Application.Modules.CustomerModification.Interfaces;
using MasterDataAutomation.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasterDataAutomation.Web.Controllers
{
    [Authorize(Roles = "Sales,Manager,Admin")]
    public class SalesDashboardController : Controller
    {
        private readonly ICustomerDraftRepository _customerDraftRepository;
        private readonly ISettingsService _settingsService;
        private readonly IHistoryService _historyService;
        private readonly ICustomerModificationRequestRepository _customerModificationRequestRepository;

        public SalesDashboardController(
            ICustomerDraftRepository customerDraftRepository,
            ISettingsService settingsService,
            IHistoryService historyService,
            ICustomerModificationRequestRepository customerModificationRequestRepository)
        {
            _customerDraftRepository = customerDraftRepository;
            _settingsService = settingsService;
            _historyService = historyService;
            _customerModificationRequestRepository = customerModificationRequestRepository;
        }

        public IActionResult Index()
        {
            var customers = _customerDraftRepository.GetAll();
            var history = _historyService.GetAll();

            ViewBag.ModificationDraftsCount = _customerModificationRequestRepository.GetDrafts().Count;
            ViewBag.ModificationRejectedCount = _customerModificationRequestRepository.GetRejected().Count;
            ViewBag.ModificationSubmittedCount = _customerModificationRequestRepository.GetSubmitted().Count;

            var model = new SalesDashboardViewModel
            {
                DraftCustomersCount = customers.Count,
                LastBpCode = _settingsService.GetLastBpCode(),
                HistoryCount = history.Count,

                SubmittedCount = _customerDraftRepository.GetSubmittedCount(),
                ApprovedCount = _customerDraftRepository.GetApprovedCount(),
                RejectedCount = _customerDraftRepository.GetRejectedCount(),

                RecentCustomers = customers
                    .TakeLast(5)
                    .Reverse()
                    .ToList()
            };

            return View(model);
        }
    }
}