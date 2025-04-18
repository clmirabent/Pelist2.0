using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static Films.Services.TmbdService;

namespace Films.Controllers
{
    public class MoviesController : Controller
    {
        private readonly TmdbService _tmdbService;

        public MoviesController(TmdbService tmdbService)
        {
            _tmdbService = tmdbService;
        }

        public async Task<IActionResult> Index()
        {
            var movies = await _tmdbService.GetPopularMovieTitlesAsync();
            return View("~/Views/Home/Index.cshtml", movies);
        }
    }
}
