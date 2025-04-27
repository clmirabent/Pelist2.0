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

namespace Films.Controllers
{
    public class FriendController : Controller
    {
        private readonly FilmsDbContext _context;

        public FriendController(FilmsDbContext context)
        {
            _context = context;
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
}
