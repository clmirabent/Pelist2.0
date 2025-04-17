using System.Diagnostics;
using Films.Models;
using Microsoft.AspNetCore.Mvc;
using static Films.Services.TmbdService;

namespace Films.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TmdbService _tmdbService;

        public HomeController(ILogger<HomeController> logger, TmdbService tmdbService)
        {
            _logger = logger;
            _tmdbService = tmdbService;
        }

        public async Task<IActionResult> Index()
        {
            var movies = await _tmdbService.GetPopularMovieTitlesAsync();
            return View(movies);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
