using System.Diagnostics;
using MasterDataAutomation.Models;
using Microsoft.AspNetCore.Mvc;

namespace MasterDataAutomation.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Upload()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {

            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("", "Please select an excel file.");

                return View();
            }

            return View();
        }
    }
}
