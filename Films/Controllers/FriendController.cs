using Films.Context;
using Films.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
}