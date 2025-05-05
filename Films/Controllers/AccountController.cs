using System.Security.Claims;
using Films.Context;
using Films.Models;
using Films.Models.APIModels;
using Films.Models.ViewModels;
using Films.Services;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Films.Controllers;

public class AccountController : Controller
{
    private readonly FilmsDbContext _context;
    private readonly TmbdService.TmdbService _tmdbService;

    public AccountController(FilmsDbContext context, TmbdService.TmdbService tmdbService)
    {
        _context = context;
        _tmdbService = tmdbService;
    }

    public async Task<IActionResult> Profile()
    {
        var id = GetUserIdFromClaims();
        
        if (id == null)
        {
            TempData["SweetAlertMessage"] = "Por favor, inicia sesión.";
            return RedirectToAction("Login", "Authentication");
        }
        
        var user = await _context.Users
            .Include(u => u.Lists)
            .ThenInclude(l => l.FkIdTypeListNavigation)
            .Include(u => u.FriendFkIdFriendNavigations)
            .ThenInclude(f => f.FkIdUserNavigation)
            .Include(u=>u.Reviews)
            .FirstOrDefaultAsync(u => u.IdUser == id);
        
        
        var typeLists = await _context.TypeLists.ToListAsync();
        if (user == null)
        {
            TempData["SweetAlertMessage"] = "Por favor, inicia sesión.";
            return RedirectToAction("Login");
        }

        // Diccionario para almacenar el título de cada película
        var movieData = new Dictionary<int, string>();

        if (user.Reviews != null)
        {
            foreach (var review in user.Reviews)
            {
                try
                {
                    // Obtener los detalles de la película mediante el servicio TMDB
                    Movie movie = await _tmdbService.GetMovieById(review.FkIdMovie);
                    movieData[review.FkIdMovie] = movie.Title;
                }
                catch (Exception ex)
                {
                    movieData[review.FkIdMovie] = "Título desconocido";
                }
            }
        }
        
        ViewBag.MovieData = movieData;
        
        var viewModel = new UserProfileViewModel
        {
            User = user,
            TypeLists = typeLists,
            Reviews = user.Reviews.ToList(),
            Friends = user.FriendFkIdFriendNavigations?.ToList() ?? new List<Friend>(),
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        // Delete authentication cookie
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        // Redirect to home
        return RedirectToAction("Index", "Home");
    }
    

    public async Task<IActionResult> Friends()
    {
        var id = GetUserIdFromClaims();
        if (id == null)
        {
            TempData["SweetAlertMessage"] = "Por favor, inicia sesión para ver a tus amigos.";
            return RedirectToAction("Login", "Authentication");
        }

        var user = await _context.Users
            .Include(n => n.FriendFkIdUserNavigations).Include(user => user.FriendFkIdFriendNavigations)
            .Where(n => n.IdUser == id).SingleOrDefaultAsync();
       
        var realFriends = user.FriendFkIdFriendNavigations.ToList();

        return View(realFriends);
        
    }
   private int? GetUserIdFromClaims()
    {
        if (int.TryParse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value, out var userId))
        {
            return userId;
        }

        return null;
    }
}

