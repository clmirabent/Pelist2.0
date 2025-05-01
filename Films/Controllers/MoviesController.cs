using Films.Context;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static Films.Services.TmbdService;
using Films.Models.ViewModels;

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
