using System.Security.Claims;
using Films.Context;

using Films.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Films.Controllers;

public class AccountController : Controller
{
    private readonly FilmsDbContext _context;

    public AccountController(FilmsDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Profile()
    {
        var id = GetUserIdFromClaims();
        
        if (id == null)
        {
            // Si no hay id en los claims, se define el mensaje para el popup
            TempData["SweetAlertMessage"] = "Por favor, inicia sesión.";
            return RedirectToAction("Login", "Authentication");
        }

        //search for the user in database and their properties
        var user = await _context.Users
            .Include(u => u.Lists)
            .ThenInclude(l => l.FkIdTypeListNavigation)
            .Include(u => u.FriendFkIdFriendNavigations)
            .FirstOrDefaultAsync(u => u.IdUser == id);

        var typeLists = await _context.TypeLists.ToListAsync();
        if (user == null)
        {
            TempData["SweetAlertMessage"] = "Por favor, inicia sesión.";
            return RedirectToAction("Login");
        }

        //var friends = await GetFriendsAsync(userId);
        var viewModel = new UserProfileViewModel
        {
            User = user,
            TypeLists = typeLists,
            //Friends = friends,
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

