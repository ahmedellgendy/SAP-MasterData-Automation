using MasterDataAutomation.Application.Dtos;
using MasterDataAutomation.Application.Interfaces.Repositories;
using MasterDataAutomation.Application.Interfaces.Services;
using MasterDataAutomation.Application.Validators;
using MasterDataAutomation.Infrastructure.Settings;
using MasterDataAutomation.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace MasterDataAutomation.Web.Controllers
{
    [Authorize(Roles = "Sales,Manager,Admin")]
    public class CustomerImportController : Controller
    {
        private readonly ICustomerImportService _customerImportService;
        private readonly ISettingsService _settingsService;
        private readonly ICustomerValidator _customerValidator;
        private readonly ICustomerDraftRepository _customerDraftRepository;
        private readonly IHistoryService _historyService;


        public CustomerImportController(ICustomerImportService customerImportService,
            ISettingsService settingsService,
            ICustomerValidator customerValidator,
            ICustomerDraftRepository customerDraftRepository,
            IHistoryService historyService)
        {
            _customerImportService = customerImportService;
            _settingsService = settingsService;
            _customerValidator = customerValidator;
            _customerDraftRepository = customerDraftRepository;
            _historyService = historyService;

        }

        [HttpGet]
        public IActionResult Upload()
        {
            var model = new UploadViewModel
            {
                LastBpCode = _settingsService.GetLastBpCode()
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Upload(UploadViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _customerImportService.GenerateSapFileAsync(
                model.File,
                model.LastBpCode);

            return File(
                result.File,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                result.FileName);
        }
        [HttpGet]
        public IActionResult UploadToDraft()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UploadToDraft(UploadViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var readResult = await _customerImportService.ReadCustomersWithErrorsAsync(model.File);

            if (readResult.Errors.Any())
            {
                TempData["UploadErrors"] = JsonSerializer.Serialize(readResult.Errors);
                TempData["Error"] = "Excel file contains validation errors.";
                return RedirectToAction(nameof(UploadToDraft));
            }

            var draftCustomers = _customerDraftRepository.GetAll();

            draftCustomers.AddRange(readResult.Customers);

            _customerDraftRepository.Save(draftCustomers);

            TempData["Success"] = $"{readResult.Customers.Count} customers uploaded to draft successfully.";

            return RedirectToAction(nameof(Draft));
        }


        [HttpGet]
        public IActionResult Create()
        {
            var model = new CreateCustomerViewModel();

            LoadDropdowns(model);

            return View(model);
        }

        [HttpPost]
        public IActionResult Create(CreateCustomerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdowns(model);
                return View(model);
            }

            var currentUser = User.Identity?.Name ?? "Unknown";

            var customer = new CustomerImportDto
            {
                Line = model.Line,
                Market = model.Market,
                Branch = model.Branch,
                SalesDistrict = model.SalesDistrict,
                CustomerType = model.CustomerType,
                CreatedBy = currentUser
            };

            Console.WriteLine($"Branch = '{customer.Branch}'");

            var errors = _customerValidator.Validate(customer, 1);

            if (errors.Any())
            {
                foreach (var error in errors)
                {
                    ModelState.AddModelError(error.Column, error.Message);
                }

                LoadDropdowns(model);

                return View(model);
            }

            if (model.Index.HasValue)
            {
                _customerDraftRepository.Update(model.Index.Value, customer);
            }
            else
            {
                var customers = _customerDraftRepository.GetAll();

                customers.Add(customer);

                _customerDraftRepository.Save(customers);
            }

            return RedirectToAction(nameof(Draft));
        }


        [HttpGet]
        public IActionResult Draft()
        {
            var customers = _customerDraftRepository.GetAll();

            return View(customers);
        }
        [HttpPost]
        public async Task<IActionResult> GenerateDraft()
        {
            var customers = _customerDraftRepository.GetAll();

            if (!customers.Any())
            {
                TempData["Error"] = "Draft is empty.";
                return RedirectToAction(nameof(Draft));
            }

            for (int i = 0; i < customers.Count; i++)
            {
                var errors = _customerValidator.Validate(customers[i], i + 1);

                if (errors.Any())
                {
                    TempData["Error"] =
                        $"Draft has invalid data at row {i + 1}: {errors.First().Message}";

                    return RedirectToAction(nameof(Draft));
                }
            }

            var result = await _customerImportService.GenerateFromDraftAsync();

            if (result.Errors.Any())
            {
                TempData["Error"] = result.Errors.First().Message;
                return RedirectToAction(nameof(Draft));
            }

            return File(result.File,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                result.FileName);
        }

        [HttpGet]
        public IActionResult PreviewDraft()
        {
            var customers = _customerDraftRepository.GetAll();

            if (!customers.Any())
            {
                TempData["Error"] = "Draft is empty.";
                return RedirectToAction(nameof(Draft));
            }

            for (int i = 0; i < customers.Count; i++)
            {
                var errors = _customerValidator.Validate(customers[i], i + 1);

                if (errors.Any())
                {
                    TempData["Error"] =
                        $"Draft has invalid data at row {i + 1}: {errors.First().Message}";

                    return RedirectToAction(nameof(Draft));
                }
            }

            return View(customers);
        }

        [HttpPost]
        public IActionResult ClearDraft()
        {
            _customerDraftRepository.Clear();

            TempData["Success"] = "Draft customers cleared successfully.";

            return RedirectToAction(nameof(Draft));
        }
        [HttpPost]
        public IActionResult SubmitDraft()
        {
            var customers = _customerDraftRepository.GetAll();

            if (!customers.Any())
            {
                TempData["Error"] = "Draft is empty.";
                return RedirectToAction(nameof(Draft));
            }

            _customerDraftRepository.SubmitDraft();

            TempData["Success"] = "Draft submitted to manager successfully. Your draft is now empty until manager approval.";

            return RedirectToAction(nameof(Draft));
        }


        [HttpPost]
        public IActionResult Delete(int index)
        {
            _customerDraftRepository.RemoveAt(index);

            return RedirectToAction(nameof(Draft));
        }


        [HttpGet]
        public IActionResult Edit(int index)
        {
            var customer = _customerDraftRepository.GetByIndex(index);

            if (customer == null)
                return RedirectToAction(nameof(Draft));

            var model = new CreateCustomerViewModel
            {
                Index = index,
                Line = customer.Line,
                Market = customer.Market,
                Branch = customer.Branch,
                SalesDistrict = customer.SalesDistrict,
                CustomerType = customer.CustomerType
            };

            LoadDropdowns(model);

            return View("Create", model);
        }



        [HttpGet]
        public IActionResult History()
        {
            var history = _historyService.GetAll()
                .OrderByDescending(x => x.Date)
                .ToList();

            return View(history);
        }
        [HttpGet]
        public IActionResult DownloadHistoryFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                TempData["Error"] = "File name is missing.";
                return RedirectToAction(nameof(History));
            }

            var safeFileName = Path.GetFileName(fileName);

            var filePath = Path.Combine(
                AppContext.BaseDirectory,
                "GeneratedFiles",
                safeFileName);

            if (!System.IO.File.Exists(filePath))
            {
                TempData["Error"] = "File not found.";
                return RedirectToAction(nameof(History));
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(
                fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                safeFileName);
        }

        [HttpGet]
        public IActionResult RejectedForReview()
        {
            var customers = _customerDraftRepository.GetRejectedForReview();

            return View(customers);
        }
        [HttpPost]
        public IActionResult MoveRejectedToDraft()
        {
            var customers = _customerDraftRepository.GetRejected();

            if (!customers.Any())
            {
                TempData["Error"] = "No rejected customers found.";
                return RedirectToAction(nameof(RejectedForReview));
            }

            _customerDraftRepository.MoveRejectedToDraft();

            TempData["Success"] = "Rejected customers moved back to draft successfully.";

            return RedirectToAction(nameof(Draft));
        }


        private void LoadDropdowns(CreateCustomerViewModel model)
        {
            model.Branches = new()
            {
                new("العاشر", "10th"),
                new("الجيزة", "Giza"),
                new("الإسكندرية", "Alex"),
                new("المنصورة", "Mansora")
            };

            model.CustomerTypes = new()
            {
                new("ثلاجة", "ثلاجة"),
                new("خاص", "خاص")
            };

            model.Lines = GetLineOptions();

        }

        private static List<LineOptionViewModel> GetLineOptions()
        {
            return new()
    {
        new LineOptionViewModel { Line = "ميامي 2", SalesDistrict = "AX0033" },
        new LineOptionViewModel { Line = "سيدي بشر", SalesDistrict = "AX0034" },
        new LineOptionViewModel { Line = "العصافرة", SalesDistrict = "AX0035" },

        new LineOptionViewModel { Line = "الدقي", SalesDistrict = "GZ0010" },
        new LineOptionViewModel { Line = "الهرم", SalesDistrict = "GZ0011" },

        new LineOptionViewModel { Line = "المنصورة 1", SalesDistrict = "MN0001" },
        new LineOptionViewModel { Line = "المنصورة 2", SalesDistrict = "MN0002" }
    };
        }
    }
}