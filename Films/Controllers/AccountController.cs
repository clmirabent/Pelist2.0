using System.Security.Claims;
using Films.Context;
using Films.Models;
using Films.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Films.Services;
using Microsoft.EntityFrameworkCore;
using Films.Context;
using Films.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Films.Controllers;

public class AccountController: Controller
{
    private readonly FilmsDbContext _context;

    private readonly ICloudinaryService _cloudinaryService;
    
    public AccountController(FilmsDbContext context, ICloudinaryService cloudinaryService)
    {
        _context = context;
        _cloudinaryService = cloudinaryService;
    }


    public async Task<IActionResult> Profile()
    {
        if (!int.TryParse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value, out var userId) || userId == null)
            return RedirectToAction("Login", "Authentication");
       
        //search for the user in database and their properties
        var user = await _context.Users
            .Include(u => u.Lists)
            .ThenInclude(l => l.FkIdTypeListNavigation)
            .Include(u => u.FriendFkIdFriendNavigations)
            .FirstOrDefaultAsync(u => u.IdUser == userId);

        var typeLists = await _context.TypeLists.ToListAsync();
        if (user == null)
        {
            return NotFound("Usuario no encontrado.");
        }

        var viewModel = new UserProfileViewModel
        {
            User = user,
            TypeLists = typeLists

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

  
}

