using Films.Context;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static Films.Services.TmbdService;

namespace Films.Controllers
{
    public class MoviesController : Controller
    {
        private readonly FilmsDbContext _context;

        private readonly TmdbService _tmdbService;

        public MoviesController(FilmsDbContext context, TmdbService tmdbService)
        {
            _tmdbService = tmdbService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var movies = await _tmdbService.GetPopularMovieTitlesAsync();
            return View("~/Views/Home/Index.cshtml", movies);
        }
    }
}
