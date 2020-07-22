using DFC.App.Pages.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace DFC.App.Pages.Controllers
{
    public class HomeController : BasePagesController<HomeController>
    {
        public const string HomeViewCanonicalName = "home";

        public HomeController(ILogger<HomeController> logger) : base(logger)
        {
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
