using System.Diagnostics;
using System.Net.NetworkInformation;
using Films.Context;
using Films.Models;
using Films.Models.APIModels;
using Films.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static Films.Services.TmbdService;
using System.Reflection.Metadata.Ecma335;

namespace Films.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly FilmsDbContext _context;
        private readonly TmdbService _tmdbService;

        public HomeController(FilmsDbContext context, ILogger<HomeController> logger, TmdbService tmdbService)
        {
            _logger = logger;
            _context = context;
            _tmdbService = tmdbService;
        }

        public async Task<IActionResult> Index()
        {
            int? genre = HttpContext.Session.GetInt32("FilterGenre");
            int? actor = HttpContext.Session.GetInt32("FilterActor");
            int? year = HttpContext.Session.GetInt32("FilterYear");
            int? duration = HttpContext.Session.GetInt32("FilterDuration");
            string? search = HttpContext.Session.GetString("Search");

            ViewBag.FilterGenre = genre;
            ViewBag.FilterActor = actor;
            ViewBag.FilterYear = year;
            ViewBag.FilterDuration = duration;
            ViewBag.Search = search;

            var allReviews = await _context.MovieReviews.ToListAsync();

            List<MovieReview> moviesReviews = allReviews;

            var allGenres = await _tmdbService.GetGenresAsync();
            List<Genre> genres = allGenres;

            var allPopularActors = await _tmdbService.GetPopularActorsAsync();
            List<People> popularActors = allPopularActors;

            if (genre == null && actor == null && year == null && duration == null && search == null) { 
                var categorizedMovies = new Dictionary<string, List<Movie>>
                {
                    { "En tendencia", await _tmdbService.GetPopularMovieAsync() },
                    { "De terror", await _tmdbService.GetMoviesByGenreAsync(27) },
                    { "De Acción", await _tmdbService.GetMoviesByGenreAsync(28) },
                    { "De Animación", await _tmdbService.GetMoviesByGenreAsync(16) },
                    { "Para partirse de risa", await _tmdbService.GetMoviesByGenreAsync(35) },
                    { "Documentales", await _tmdbService.GetMoviesByGenreAsync(99) },
                    { "De romance", await _tmdbService.GetMoviesByGenreAsync(10749) },
                    { "De ciencia ficción", await _tmdbService.GetMoviesByGenreAsync(878) },
                    { "Para ver en família", await _tmdbService.GetMoviesByGenreAsync(10751) },
                    { "De crimenes", await _tmdbService.GetMoviesByGenreAsync(80) },
                };

                foreach (var movieList in categorizedMovies.Values)
                {
                    AddReviewsToMovies(movieList, moviesReviews);
                }

                var userIdClaim = User.FindFirst("UserId");
                int idUser = userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;

                var userLists = await _context.Lists
                    .Where(l => l.FkIdUser == idUser)
                    .ToListAsync();

                var viewModel = new HomeViewModel
                {
                    CategorizedMovies = categorizedMovies,
                    UserMovieLists = userLists,
                    Genres = genres,
                    Actors = popularActors,
                };
                return View(viewModel);
            } 
            else
            {
                var filteredMovies = await _tmdbService.GetMoviesAsync(2);

                // Filtrar por género
                if (genre.HasValue && genre.Value != 0)
                {
                    filteredMovies = filteredMovies
                    .Where(m => m.Genres.Any(g => g.Id == genre.Value))
                    .ToList();
                }

                // Filtrar por año
                if (year.HasValue && year.Value != 0)
                {
                    filteredMovies = filteredMovies
                    .Where(m =>
                        !string.IsNullOrEmpty(m.ReleaseDate) &&
                        DateTime.TryParse(m.ReleaseDate, out var date) &&
                        date.Year == year.Value
                    ).ToList();
                }

                // Filtrar por duración
                if (duration.HasValue && duration.Value != 0)
                {
                    filteredMovies = await FilterByDuration(filteredMovies, duration.Value);
                }

                // Filtrar por actor
                if (actor.HasValue && actor.Value != 0)
                {
                    filteredMovies = await FilterByActor(filteredMovies, actor.Value);
                }

                // Filtrar por título
                if (!string.IsNullOrEmpty(search))
                {
                    filteredMovies = filteredMovies
                        .Where(m => m.Title != null && m.Title.Contains(search, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                // Añadir valoraciones
                AddReviewsToMovies(filteredMovies, moviesReviews);

                var userIdClaim = User.FindFirst("UserId");
                int idUser = userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;

                var userLists = await _context.Lists
                    .Where(l => l.FkIdUser == idUser)
                    .ToListAsync();

                filteredMovies = filteredMovies.Take(50).ToList();

                var viewModel = new HomeViewModel
                {
                    CategorizedMovies = new Dictionary<string, List<Movie>>
                    {
                        { "Resultados del filtro", filteredMovies }
                    },
                    UserMovieLists = userLists,
                    Genres = genres,
                    Actors = popularActors,
                };

                return View(viewModel);
            }
        }

        private async Task<List<Movie>> FilterByDuration(List<Movie> movies, int maxDuration)
        {
            var result = new List<Movie>();
            foreach (var movie in movies)
            {
                var details = await _tmdbService.GetMovieById(movie.Id);
                if (details != null && details.Runtime <= maxDuration)
                {
                    result.Add(movie);
                }
            }
            return result;
        }

        private async Task<List<Movie>> FilterByActor(List<Movie> movies, int actorId)
        {
            var result = new List<Movie>();
            foreach (var movie in movies)
            {
                var cast = await _tmdbService.GetMovieCastAsync(movie.Id);
                if (cast.Any(c => c.Id == actorId))
                {
                    result.Add(movie);
                }
            }
            return result;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        private void AddReviewsToMovies(List<Movie> movies, List<MovieReview> reviews)
        {
            foreach (var movie in movies)
            {
                var review = reviews.FirstOrDefault(r => r.FkIdMovie == movie.Id);
                movie.Review = review?.AverageRating ?? 0;
            }
        }

        public async Task<IActionResult> AddToList(string listType, int idFilm)
        {
            var userIdClaim = User.FindFirst("UserId");
            int idUser = userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;

            int idTypeList = _context.TypeLists
                .FirstOrDefault(t => t.ListName == listType)?.IdListType ?? 0;

            var existingEntry = _context.Lists
                .FirstOrDefault(l => l.FkIdUser == idUser && l.FkIdMovie == idFilm);

            if (existingEntry != null)
            {
                if (existingEntry.FkIdTypeList == idTypeList)
                {
                    _context.Lists.Remove(existingEntry);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }

                _context.Lists.Remove(existingEntry);
                await _context.SaveChangesAsync();
            }

            _context.Lists.Add(new List
            {
                FkIdUser = idUser,
                FkIdMovie = idFilm,
                FkIdTypeList = idTypeList
            });

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddFilters(int genre, int actor, int year, int duration)
        {
            HttpContext.Session.SetInt32("FilterGenre", genre);
            HttpContext.Session.SetInt32("FilterActor", actor);
            HttpContext.Session.SetInt32("FilterYear", year);
            HttpContext.Session.SetInt32("FilterDuration", duration);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ResetFilters()
        {
            HttpContext.Session.Remove("FilterGenre");
            HttpContext.Session.Remove("FilterActor");
            HttpContext.Session.Remove("FilterYear");
            HttpContext.Session.Remove("FilterDuration");
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetSearchMovies(string txtSearch)
        {
            if (txtSearch != null)
            {
                if (!txtSearch.Equals(""))
                    HttpContext.Session.SetString("Search", txtSearch);
                else
                    HttpContext.Session.Remove("Search");
            }
            else
                HttpContext.Session.Remove("Search");

            return RedirectToAction("Index", "Home");
        }
    }
}
