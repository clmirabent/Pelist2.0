using Films.Context;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static Films.Services.TmbdService;
using Films.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Films.Models;

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

        [Route("movie/{id}/detail")] // Ruta personalizada para la acción Detail cuando se hace clic en una movie-card
        public async Task<IActionResult> Detail(int id)
        {
            var movie = await _tmdbService.GetMovieById(id);

            if (movie == null)
                return NotFound();

            var userIdClaim = User.FindFirst("UserId");
            int idUser = userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;

            var userLists = await _context.Lists
                .Where(l => l.FkIdUser == idUser)
                .ToListAsync();

            // Recuperar la nota de review promedio de esta peli
            var review = _context.MovieReviews.FirstOrDefault(r => r.FkIdMovie == movie.Id);
            movie.Review = review?.AverageRating ?? 0;

            var genreIds = movie.Genres.Select(g => g.Id).ToList();
            int genreIdToUse = genreIds.FirstOrDefault();

            var relatedMovies = (await _tmdbService.GetMoviesByGenreAsync(genreIdToUse))
                .Where(m => m.Id != movie.Id)
                .Take(30) // limitar la cantidad
                .ToList();

            // Obtener la lista de actores
            var actors = await _tmdbService.GetPopularActorsByMovieId(movie.Id);
            movie.Persons = actors;

            var vm = new MovieDetailsViewModel
            {
                Id = movie.Id,
                Title = movie.Title,
                Genres = movie.Genres,
                Review = movie.Review,
                Overview = movie.Overview,
                UserMovieLists = userLists,
                PosterPath = movie.PosterPath,
                BackdropPath = movie.BackdropPath,
                ReleaseDate = DateTime.Parse(movie.ReleaseDate),
                RelatedMovies = relatedMovies,
                Persons = movie.Persons
            };

            return View(vm);
        }

        //BORRAR ESTO DESPUÉS DE PASAR EL FORMULARIO A LA VISTA DE DETALLE

                                    [Route("movie/{id}/mr")]
                                    public async Task<IActionResult> FormReview(int id)
                                    {
                                        var movie = await _tmdbService.GetMovieById(id);

                                        if (movie == null)
                                            return NotFound();

                                        var vm = new MovieDetailsViewModel
                                        {
                                            Id = movie.Id,
                                            Title = movie.Title,
                                            Genres = movie.Genres,
                                            Review = movie.Review,
                                            Overview = movie.Overview,
                                            PosterPath = movie.PosterPath,
                                            BackdropPath = movie.BackdropPath,
                                            ReleaseDate = DateTime.Parse(movie.ReleaseDate),
                                        };

                                        return View(vm);
                                    }

        //BORRAR 

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddReview(string titleReview, string descriptionReview, int ratingInput, int idFilm)
        {
            if (titleReview != null)
            {
                
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
