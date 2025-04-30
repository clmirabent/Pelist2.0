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

        // Incluimos ambas colecciones para las relaciones de amistad
        var currentUser = await _context.Users
            .Include(u => u.FriendFkIdUserNavigations)
            .ThenInclude(f => f.FkIdFriendNavigation)
            .Include(u => u.FriendFkIdFriendNavigations)
            .ThenInclude(f => f.FkIdUserNavigation)
            .SingleOrDefaultAsync(u => u.IdUser == id);

        // Amistades donde el usuario es receptor (obtenemos al amigo mediante FkIdFriendNavigation)
        var friendsReceived = currentUser?.FriendFkIdUserNavigations
            .Where(f => !f.PendingFriend)
            .Select(f => f.FkIdFriendNavigation)
            .ToList() ?? new List<User>();

        // Amistades donde el usuario es emisor (en este caso, el amigo es obtenido mediante FkIdUserNavigation)
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
    
    // Primero, obtenemos los id's de relaciones existentes (aceptadas y pendientes)
    var relatedFriendIdsQuery = _context.Friends
        .Where(f => f.FkIdUser == id || f.FkIdFriend == id)
        .Select(f => new { f.FkIdUser, f.FkIdFriend });
    
    var relatedFriendIdsList = await relatedFriendIdsQuery.ToListAsync();
    
    // Creamos una lista de ID a excluir (incluyendo el ID actual)
    var excludeIds = new List<int> { id.Value };
    foreach (var relation in relatedFriendIdsList)
    {
        if (!excludeIds.Contains(relation.FkIdUser))
            excludeIds.Add(relation.FkIdUser);
        if (!excludeIds.Contains(relation.FkIdFriend))
            excludeIds.Add(relation.FkIdFriend);
    }
    
    // Buscamos usuarios cuyo Username contenga el término y que no estén en la lista de excluidos
    var users = await _context.Users
        .Where(u => !excludeIds.Contains(u.IdUser) &&
                    (searchUser == null || u.Username.Contains(searchUser)))
        .ToListAsync();
    
    // Obtenemos nuevamente al usuario actual con todas las relaciones para sus amigos aceptados
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
        SearchResults = users,
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
        
        return RedirectToAction("SearchUsers", new { searchUser = "" });
        
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

