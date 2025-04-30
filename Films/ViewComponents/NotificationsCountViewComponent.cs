using Films.Context;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

public class NotificationsCountViewComponent : ViewComponent
{
    private readonly FilmsDbContext _context;

    public NotificationsCountViewComponent(FilmsDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var userIdClaim = HttpContext.User.FindFirst("UserId");
        int idUser = userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;

        var count = await _context.Friends
            .Where(f => f.FkIdUser == idUser && f.PendingFriend == true)
            .CountAsync();

        return View(count);
    }
}
