using MasterDataAutomation.Application.Interfaces.Services;
using MasterDataAutomation.Infrastructure.Data;
using MasterDataAutomation.Infrastructure.Data.Entities;
using MasterDataAutomation.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace MasterDataAutomation.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminUsersController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IActivityLogService _activityLogService;

    public AdminUsersController(
        ApplicationDbContext context,
        IActivityLogService activityLogService)
    {
        _context = context;
        _activityLogService = activityLogService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var users = _context.AppUsers
            .OrderBy(x => x.Role)
            .ThenBy(x => x.FullName)
            .ToList();

        return View(users);
    }

    [HttpGet]
    public IActionResult Create()
    {
        var model = new CreateUserViewModel();
        LoadRoles(model);

        return View(model);
    }

    [HttpPost]
    public IActionResult Create(CreateUserViewModel model)
    {
        if (!ModelState.IsValid)
        {
            LoadRoles(model);
            return View(model);
        }

        var exists = _context.AppUsers
            .Any(x => x.UserName == model.UserName);

        if (exists)
        {
            ModelState.AddModelError(nameof(model.UserName), "Username already exists.");
            LoadRoles(model);
            return View(model);
        }

        var user = new AppUserEntity
        {
            FullName = model.FullName,
            UserName = model.UserName,
            Password = model.Password,
            Role = model.Role,
            IsActive = true,
            CreatedAt = DateTime.Now
        };

        _context.AppUsers.Add(user);
        _context.SaveChanges();

        _activityLogService.Log(
            action: "Create",
            module: "Administration",
            entityName: "User",
            entityId: user.Id,
            description: $"Created user {user.UserName} with role {user.Role}");

        TempData["Success"] = "User created successfully.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult ToggleStatus(int id)
    {
        var user = _context.AppUsers.FirstOrDefault(x => x.Id == id);

        if (user == null)
        {
            TempData["Error"] = "User not found.";
            return RedirectToAction(nameof(Index));
        }

        user.IsActive = !user.IsActive;

        _context.SaveChanges();

        _activityLogService.Log(
            action: user.IsActive ? "Activate" : "Deactivate",
            module: "Administration",
            entityName: "User",
            entityId: user.Id,
            description: $"{(user.IsActive ? "Activated" : "Deactivated")} user {user.UserName} with role {user.Role}");

        TempData["Success"] = "User status updated successfully.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        var currentUserIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (int.TryParse(currentUserIdValue, out var currentUserId) && currentUserId == id)
        {
            TempData["Error"] = "You cannot delete your own account while logged in.";
            return RedirectToAction(nameof(Index));
        }

        var user = _context.AppUsers.FirstOrDefault(x => x.Id == id);

        if (user == null)
        {
            TempData["Error"] = "User not found.";
            return RedirectToAction(nameof(Index));
        }

        var deletedUserName = user.UserName;
        var deletedUserRole = user.Role;
        var deletedUserFullName = user.FullName;

        _context.AppUsers.Remove(user);
        _context.SaveChanges();

        _activityLogService.Log(
            action: "Delete",
            module: "Administration",
            entityName: "User",
            entityId: id,
            description: $"Deleted user {deletedUserName} - {deletedUserFullName} with role {deletedUserRole}");

        TempData["Success"] = "User deleted successfully.";

        return RedirectToAction(nameof(Index));
    }

    private void LoadRoles(CreateUserViewModel model)
    {
        model.Roles = new()
    {
        new SelectListItem("Sales", "Sales"),
        new SelectListItem("Manager", "Manager"),

        new SelectListItem
        {
            Text = "Accounts User",
            Value = "AccountsUser"
        },

        new SelectListItem
        {
            Text = "Accounts Manager",
            Value = "AccountsManager"
        },

        // Customer Care
        new SelectListItem
        {
            Text = "Customer Care Agent",
            Value = "ComplaintAgent"
        },

        new SelectListItem
        {
            Text = "Customer Care Supervisor",
            Value = "ComplaintSupervisor"
        },

        new SelectListItem
        {
            Text = "Customer Care Manager",
            Value = "ComplaintManager"
        },

        // Product pricing executive role
        new SelectListItem
        {
            Text = "Executive Manager - Pricing",
            Value = "ExecutiveManager"
        },

        // CEO dashboard only role
        new SelectListItem
        {
            Text = "CEO",
            Value = "CEO"
        },

        new SelectListItem("Admin", "Admin")
    };
    }
}