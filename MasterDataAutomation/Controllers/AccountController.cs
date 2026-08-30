using System.Security.Claims;
using MasterDataAutomation.Infrastructure.Data;
using MasterDataAutomation.Web.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace MasterDataAutomation.Web.Controllers;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;

    public AccountController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View(new LoginViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = _context.AppUsers
            .FirstOrDefault(x =>
                x.UserName == model.UserName &&
                x.Password == model.Password &&
                x.IsActive);

        if (user == null)
        {
            ModelState.AddModelError("", "Invalid username or password.");
            return View(model);
        }

        var userRole = user.Role?.Trim() ?? string.Empty;

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim("FullName", user.FullName),
            new Claim(ClaimTypes.Role, userRole)
        };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal);

        return RedirectToHomeByRole(userRole);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            var role =
                User.FindFirstValue(
                    ClaimTypes.Role)
                ?.Trim()
                ?? string.Empty;

            return RedirectToHomeByRole(role);
        }

        return View();
    }

    private IActionResult RedirectToHomeByRole(string role)
    {
        return role switch
        {
            "Admin" => RedirectToAction("Index", "ManagerDashboard"),
            "Manager" => RedirectToAction("Index", "ManagerDashboard"),
            "Sales" => RedirectToAction("Index", "SalesDashboard"),
            "AccountsUser" => RedirectToAction("Index", "ProductRequests"),
            "AccountsManager" => RedirectToAction("Index", "ProductAccountsManager"),
            "ExecutiveManager" => RedirectToAction("Index", "ProductExecutiveManager"),
            "Executive Manager" => RedirectToAction("Index", "SalesAnalyticsDashboard"),
            "CEO" => RedirectToAction("Index", "SalesAnalyticsDashboard"),

            "ComplaintAgent" => RedirectToAction("Index", "CustomerCare"),
            "ComplaintSupervisor" => RedirectToAction("Dashboard", "CustomerCare"),
            "ComplaintManager" => RedirectToAction("Dashboard", "CustomerCare"),

            _ => RedirectToAction("AccessDenied", "Account")
        };
    }
}