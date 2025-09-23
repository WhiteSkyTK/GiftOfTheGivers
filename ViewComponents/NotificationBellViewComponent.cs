using Gift_Of_The_Givers_Web_App.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Gift_Of_The_Givers_Web_App.Models;

namespace Gift_Of_The_Givers_Web_App.ViewComponents
{
    public class NotificationBellViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public NotificationBellViewComponent(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = _userManager.GetUserId((ClaimsPrincipal)User);
            if (string.IsNullOrEmpty(userId))
            {
                return Content(""); // Don't show anything if user is not logged in
            }

            var unreadCount = await _context.UserNotifications
                .CountAsync(un => un.UserID == userId && !un.IsRead);

            return View(unreadCount); // Pass the count to the view
        }
    }
}