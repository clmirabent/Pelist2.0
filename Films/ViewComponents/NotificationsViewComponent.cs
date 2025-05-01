using Films.Context;
using Films.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class NotificationsViewComponent : ViewComponent
{
    private readonly FilmsDbContext _context;

    public NotificationsViewComponent(FilmsDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var userIdClaim = HttpContext.User.FindFirst("UserId");
        int idUser = userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;

        var pendingFriendIds = await _context.Friends
            .Where(f => f.FkIdUser == idUser && f.PendingFriend)
            .Select(f => f.FkIdFriend)
            .ToListAsync();

        var pendingFriends = await _context.Users
            .Where(u => pendingFriendIds.Contains(u.IdUser))
            .Select(u => new FriendRequestViewModel
            {
                IdUser = u.IdUser,
                Username = u.Username,
                Image = u.Image
            })
            .ToListAsync();

        return View(pendingFriends);
    }
}
