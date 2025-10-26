using Gift_Of_The_Givers_Web_App.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Gift_Of_The_Givers_Web_App.Controllers
{
    /*public class SetupController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SetupController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> PromoteToAdmin()
        {
            // Find the user you just registered
            var user = await _userManager.FindByEmailAsync("admin@giftofthegivers.com");
            if (user != null)
            {
                // Assign the 'Admin' role to the user
                await _userManager.AddToRoleAsync(user, "Admin");
                return Content("User has been promoted to Admin.");
            }
            return Content("User not found.");
        }
    }*/
}
