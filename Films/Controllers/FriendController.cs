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

namespace Films.Controllers;

public class FriendController : Controller
{
    private readonly FilmsDbContext _context;

    public FriendController(FilmsDbContext context)
    {
        _context = context;
    }


    public int? GetUserIdFromClaims()
    {
        if (int.TryParse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value, out var userId))
        {
            return userId;
        }

        return null;
    }

    // Display the friend's list
    public async Task<IActionResult> Friends()
    {
        var id = GetUserIdFromClaims();
        if (id == null)
        {
            TempData["SweetAlertMessage"] = "Por favor, inicia sesión para ver a tus amigos.";
            return RedirectToAction("Login", "Authentication");
        }

        var currentUser = await _context.Users
            .Include(u => u.FriendFkIdUserNavigations)
            .ThenInclude(f => f.FkIdFriendNavigation)
            .Include(u => u.FriendFkIdFriendNavigations)
            .SingleOrDefaultAsync(u => u.IdUser == id);

        var friendUsers = currentUser?.FriendFkIdUserNavigations
            .Where(f => !f.PendingFriend) // Se consideran amigos solo cuando PendingFriend es false (aceptado)
            .Select(f => f.FkIdFriendNavigation)
            .ToList();

        return View("Friends", friendUsers);
    }

    // SearchUsers

    [HttpGet]
    public async Task<IActionResult> SearchUsers(string searchUser)
    {
        var id = GetUserIdFromClaims();
        if (id == null)
        {
            TempData["SweetAlertMessage"] = "Por favor, inicia sesión para buscar amigos";
            return RedirectToAction("Login", "Authentication");
        }

        var userSet = _context.Users;
        var users = await userSet
            .Where(u =>
                u.IdUser != id && u.FriendFkIdFriendNavigations.All(f => f.FkIdFriend != id) &&
                u.Username.Contains(searchUser ?? ""))
            .ToListAsync();
        
        ViewBag.users = users;

        return View("Friends", users);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task <IActionResult> SendFriendRequest(int targetUserId)
    {
        var currentUserId = GetUserIdFromClaims();
        if (targetUserId == currentUserId)
        {
            return BadRequest("No puedes enviarte una solicitud a ti mismo.");
        }

        var existingFriendRequest = await _context.Friends.AnyAsync(f =>
            f.FkIdUser == currentUserId && f.FkIdFriend == targetUserId ||
            f.FkIdUser == targetUserId && f.FkIdFriend == currentUserId);
        ;
        if (!existingFriendRequest)
        {
            var friendRequest = new Friend()
            {
                FkIdUser = (int)currentUserId,
                FkIdFriend = (int)targetUserId,
                PendingFriend = false,
            };
            _context.Friends.Add(friendRequest);
            await _context.SaveChangesAsync();
        }
        
        

        return RedirectToAction("SearchUsers", new { searchUser = "" });

        // RemoveFriend, SendFriendRequest, etc.
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HandleFriendRequest(int friendId, string actionType)
    {
        var userIdClaim = HttpContext.User.FindFirst("UserId");
        int idUser = userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;

        var friendRequest = await _context.Friends
            .FirstOrDefaultAsync(f => f.FkIdUser == idUser && f.FkIdFriend == friendId && f.PendingFriend == true);

        if (friendRequest == null)
        {
            return NotFound();
        }

        if (actionType == "accept")
        {
            friendRequest.PendingFriend = false;
            _context.Friends.Update(friendRequest);
        }
        else if (actionType == "reject")
        {
            _context.Friends.Remove(friendRequest);
        }

        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Home");

    }
}

