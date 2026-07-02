using MasterDataAutomation.Application.Interfaces.Repositories;
using MasterDataAutomation.Application.Interfaces.Services;
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

        public SalesDashboardController(ICustomerDraftRepository customerDraftRepository, ISettingsService settingsService, IHistoryService historyService)
        {
            _customerDraftRepository = customerDraftRepository;
            _settingsService = settingsService;
            _historyService = historyService;
        }

        public IActionResult Index()
        {
            var customers = _customerDraftRepository.GetAll();
            var history = _historyService.GetAll();

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