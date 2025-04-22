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

            var allReviews = await _context.MovieReviews.ToListAsync();

            List<MovieReview> moviesReviews = allReviews;

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
                UserMovieLists = userLists
            };

            return View(viewModel);
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
    }
}
