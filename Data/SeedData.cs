using Gift_Of_The_Givers_Web_App.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Gift_Of_The_Givers_Web_App.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // --- Seed Roles ---
            string[] roleNames = { "Admin", "Volunteer", "GeneralUser" };
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // --- Seed Resources ---
            // Look for any resources.
            if (!context.Resources.Any())
            {
                context.Resources.AddRange(
                    new Resource { ResourceName = "Blankets", Description = "Warm blankets for cold weather." },
                    new Resource { ResourceName = "Canned Food", Description = "Non-perishable food items." },
                    new Resource { ResourceName = "Bottled Water", Description = "Clean drinking water." },
                    new Resource { ResourceName = "First Aid Kits", Description = "Basic medical supplies." },
                    new Resource { ResourceName = "Hygiene Packs", Description = "Soap, toothbrushes, sanitary items." }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}