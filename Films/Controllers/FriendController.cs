using Films.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Films.Controllers;

public class FriendController: Controller
{ 
    private readonly FilmsDbContext _context;
    
        public FriendController(FilmsDbContext context)
        {
            _context = context;
        }
    
        // Muestra la lista de amigos aceptados
        
        public async Task<IActionResult> Friends()
        {
            var id = GetUserIdFromClaims();
            if (id == null)
            {
                TempData["SweetAlertMessage"] = "Por favor, inicia sesión para ver a tus amigos.";
                return RedirectToAction("Login", "Authentication");
            }

            var user = await _context.Users
                .Include(u => u.FriendFkIdUserNavigations)
                .ThenInclude(f => f.FkIdFriendNavigation)
                .Include(u => u.FriendFkIdFriendNavigations) 
                .SingleOrDefaultAsync(u => u.IdUser == id);
       
            var friendUsers = user.FriendFkIdUserNavigations
                .Where(f => !f.PendingFriend) // amistades aceptadas
                .Select(f => f.FkIdFriendNavigation)
                .ToList();

            return View(friendUsers);
        }
        
        public int? GetUserIdFromClaims()
        {
            if (int.TryParse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value, out var userId))
            {
                return userId;
            }

            return null;
        }
    
        // Otras acciones: SearchFriend, RemoveFriend, SendFriendRequest, etc.
    }
    