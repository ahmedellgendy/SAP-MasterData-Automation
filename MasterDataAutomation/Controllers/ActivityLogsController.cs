using MasterDataAutomation.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MasterDataAutomation.Web.Controllers;

[Authorize(Roles = "Admin")]
public class ActivityLogsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ActivityLogsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index(string? searchTerm, string? module, string? action)
    {
        var query = _context.ActivityLogs.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.Trim();

            query = query.Where(x =>
                (x.UserName != null && x.UserName.Contains(searchTerm)) ||
                (x.UserRole != null && x.UserRole.Contains(searchTerm)) ||
                x.Action.Contains(searchTerm) ||
                x.Module.Contains(searchTerm) ||
                x.EntityName.Contains(searchTerm) ||
                (x.Description != null && x.Description.Contains(searchTerm)));
        }

        if (!string.IsNullOrWhiteSpace(module))
        {
            query = query.Where(x => x.Module == module);
        }

        if (!string.IsNullOrWhiteSpace(action))
        {
            query = query.Where(x => x.Action == action);
        }

        var logs = query
            .OrderByDescending(x => x.CreatedAt)
            .Take(300)
            .ToList();

        ViewBag.SearchTerm = searchTerm;
        ViewBag.SelectedModule = module;
        ViewBag.SelectedAction = action;

        ViewBag.Modules = _context.ActivityLogs
            .Select(x => x.Module)
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        ViewBag.Actions = _context.ActivityLogs
            .Select(x => x.Action)
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        return View(logs);
    }
}