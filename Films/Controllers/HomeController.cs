using System.Diagnostics;
using Films.Models;
using Microsoft.AspNetCore.Mvc;

namespace Films.Controllers
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
            //return View();
            string username = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Login", "Authentication");

            ViewBag.Username = username;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
