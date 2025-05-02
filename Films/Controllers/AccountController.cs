using System.Security.Claims;
using Films.Context;
using Films.Models;
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
        
        var viewModel = new UserProfileViewModel
        {
            User = user,
            TypeLists = typeLists,
            Friends = await GetFriends(user.IdUser),
            FriendRequests = await GetFriendRequestReceived(user.IdUser)
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
    
    public async Task<List<Friend>> GetFriendRequestReceived(int userId)
    {
        var friendRequestPending = await _context.Friends
            .Where(f => f.FkIdUser == userId && f.PendingFriend == true)
            .Include(friend => friend.FkIdFriendNavigation)
            .ToListAsync();
        return friendRequestPending;
    }

    public async Task<List<Friend>> GetFriends(int userId)
    {
        return await _context.Friends
            .Where(f => (f.FkIdFriend == userId || f.FkIdUser == userId) && f.PendingFriend == false)
            .ToListAsync();
    }

    public int? GetUserIdFromClaims()
    {
        if (int.TryParse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value, out var userId))
        {
            return userId;
        }

        return null;
    }
}

