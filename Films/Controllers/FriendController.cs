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
            .ThenInclude(f => f.FkIdUserNavigation)
            .SingleOrDefaultAsync(u => u.IdUser == id);

        // Amistades donde el usuario es receptor
        var friendsReceived = currentUser?.FriendFkIdUserNavigations
            .Where(f => !f.PendingFriend)
            .Select(f => f.FkIdFriendNavigation)
            .ToList() ?? new List<User>();

        // Amistades donde el usuario es emisor 
        var friendsSent = currentUser?.FriendFkIdFriendNavigations
            .Where(f => !f.PendingFriend)
            .Select(f => f.FkIdUserNavigation)
            .ToList() ?? new List<User>();

        // Unión de ambas listas
        var acceptedFriends = friendsReceived.Union(friendsSent).ToList();

        var model = new FriendViewModel
        {
            Friends = acceptedFriends,
            SearchResults = new List<User>(), // Vacío cuando solo se muestran "Mis Amigos"
            SearchTerm = string.Empty
        };

        return View("Friends", model);
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
    
   
    // Obtener las relaciones existentes del usuario actual
    var friendRelations = await _context.Friends
        .Where(f => f.FkIdUser == id || f.FkIdFriend == id)
        .ToListAsync();
    
    // Buscar todos los usuarios que coincidan con el término y que no sean el usuario actual
    var searchResults = await _context.Users
        .Where(u => u.IdUser != id && (searchUser == null || u.Username.Contains(searchUser)))
        .ToListAsync();
    
    // Asignar el estado de la amistad a cada usuario encontrado
    foreach (var user in searchResults)
    {
        // Busca la relación existente (si existe) para el usuario en cuestión
        var relation = friendRelations.FirstOrDefault(r => r.FkIdUser == user.IdUser || r.FkIdFriend == user.IdUser);
        if (relation != null)
        {
            // Si la solicitud está pendiente, se marca como "Solicitud enviada"; de lo contrario, como "Amigo"
            user.FriendshipStatus = relation.PendingFriend ? "Solicitud enviada" : "Amigo";
        }
        else
        {
            user.FriendshipStatus = ""; // Sin relación, se deja vacío (o podrías asignar "Sin agregar")
        }
    }
    
    // Manejar el caso de que no se encuentren usuarios
    if (!searchResults.Any())
    {
        TempData["ErrorMessage"] = "El usuario no existe.";
    }
    
    //  usuario actual con todas las relaciones para sus amigos aceptados
    var currentUser = await _context.Users
        .Include(u => u.FriendFkIdUserNavigations)
            .ThenInclude(f => f.FkIdFriendNavigation)
        .Include(u => u.FriendFkIdFriendNavigations)
            .ThenInclude(f => f.FkIdUserNavigation)
        .SingleOrDefaultAsync(u => u.IdUser == id);
    
    var friendsReceived = currentUser?.FriendFkIdUserNavigations
        .Where(f => !f.PendingFriend)
        .Select(f => f.FkIdFriendNavigation)
        .ToList() ?? new List<User>();
    
    var friendsSent = currentUser?.FriendFkIdFriendNavigations
        .Where(f => !f.PendingFriend)
        .Select(f => f.FkIdUserNavigation)
        .ToList() ?? new List<User>();

    var acceptedFriends = friendsReceived.Union(friendsSent).ToList();
    
    var model = new FriendViewModel
    {
        Friends = acceptedFriends,
        SearchResults = searchResults,
        SearchTerm = searchUser
    };

    return View("Friends", model);
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
            f.FkIdUser == targetUserId && f.FkIdFriend == currentUserId ||  f.FkIdUser == currentUserId && f.FkIdFriend == targetUserId)
        ;
        if (!existingFriendRequest)
        {
            var friendRequest = new Friend()
            {
                FkIdUser = (int)targetUserId, //esto es el que recibe la solicitud de amistad
                FkIdFriend = (int)currentUserId, //esto es el que envia la solicitud de amistad 
                PendingFriend = true,
            };
            _context.Friends.Add(friendRequest);
            await _context.SaveChangesAsync();
        }
        
        return RedirectToAction("Friends", "Friend");
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HandleFriendRequest(int friendId, string actionType)
    {
        var userIdClaim = GetUserIdFromClaims();

        var friendRequest = await _context.Friends
            .FirstOrDefaultAsync(f => f.FkIdFriend == friendId && f.FkIdUser == userIdClaim && f.PendingFriend == true);

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
        
        var friendName = await _context.Users.FirstOrDefaultAsync(u => u.IdUser == friendId);
        
        // Popups
        if (actionType == "accept")
        {
            TempData["FriendshipMessage"] = $"{friendName.Username} y tú ahora son amigos.";
        }
        else if (actionType == "reject")
        {
            TempData["FriendshipMessage"] = $"Has rechazado la solicitud de amistad de {friendName.Username}.";
        }

        return RedirectToAction("Friends", "Friend");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteFriend(int friendId)
    {
        var userIdClaim = GetUserIdFromClaims();
        if (userIdClaim == null)
        {
            return BadRequest("Usuario no autenticado");
        }

        // Busca la relación de amistad en la tabla Friends
        
        var friendRequest = await _context.Friends.FirstOrDefaultAsync(f =>
            (f.FkIdUser == userIdClaim && f.FkIdFriend == friendId) ||
            (f.FkIdUser == friendId && f.FkIdFriend == userIdClaim));

        if (friendRequest == null)
        {
            return NotFound("No se encontró la solicitud de amistad.");
        }
        
        var friendName = await _context.Users.FirstOrDefaultAsync(u => u.IdUser == friendId);


        // Elimina la solicitud de amistad
        _context.Friends.Remove(friendRequest);
        await _context.SaveChangesAsync();
        
        // Asignamos el mensaje a TempData para que la vista lo pueda utilizar y mostrar el SweetAlert2
        TempData["SweetAlertMessage"] = $"Has eliminado a {friendName.Username} de tus amigos.";
        return RedirectToAction("Friends");
    }

    
}

