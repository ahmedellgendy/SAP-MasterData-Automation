using MasterDataAutomation.Application.Interfaces.Services;
using MasterDataAutomation.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasterDataAutomation.Web.Controllers;

[Authorize(Roles = "Admin")]
public class SystemSettingsController : Controller
{
    private readonly ISettingsService _settingsService;

    public SystemSettingsController(ISettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var model = new SystemSettingsViewModel
        {
            LastBpCode = _settingsService.GetLastBpCode()
        };

        return View(model);
    }

    [HttpPost]
    public IActionResult Index(SystemSettingsViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        _settingsService.SaveLastBpCode(model.LastBpCode);

        TempData["Success"] = "Last BP Code updated successfully.";

        return RedirectToAction(nameof(Index));
    }
}