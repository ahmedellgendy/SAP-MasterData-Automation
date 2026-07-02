using MasterDataAutomation.Infrastructure.Data;
using MasterDataAutomation.Infrastructure.Data.Entities;
using MasterDataAutomation.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MasterDataAutomation.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminUsersController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminUsersController(ApplicationDbContext context)
    {
        _context = context;
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

        TempData["Success"] = "User created successfully.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult ToggleStatus(int id)
    {
        var user = _context.AppUsers.FirstOrDefault(x => x.Id == id);

        if (user == null)
            return RedirectToAction(nameof(Index));

        user.IsActive = !user.IsActive;

        _context.SaveChanges();

        TempData["Success"] = "User status updated successfully.";

        return RedirectToAction(nameof(Index));
    }

    private void LoadRoles(CreateUserViewModel model)
    {
        model.Roles = new()
        {
            new SelectListItem("Sales", "Sales"),
            new SelectListItem("Manager", "Manager"),
            new SelectListItem("Admin", "Admin")
        };
    }
}