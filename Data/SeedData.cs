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

             /*if (!context.DisasterIncidents.Any())
             {
                 context.DisasterIncidents.AddRange(
                     new DisasterIncident { Title = "KZN Coastal Flood Relief", Location = "North Coast, KwaZulu-Natal", ImageUrl = "https://i0.wp.com/www.dailymaverick.co.za/wp-content/uploads/2022/08/MC-CSW-29Aug.jpg?resize=1440%2C720&quality=89&ssl=1", Description = "Following torrential rains, many coastal communities have been devastated by flash floods. Our teams are on the ground providing immediate aid, including warm meals, blankets, and temporary shelter for displaced families.", Status = "Active", FundingGoal = 50000, CurrentFunds = 12000, IncidentDate = DateTime.UtcNow },
                     new DisasterIncident { Title = "Western Cape Wildfire Support", Location = "Boland, Western Cape", ImageUrl = "https://cdn.24.co.za/files/Cms/General/d/9639/220ca2d6b50449569d30d4570c3c8f09.jpg", Description = "Fast-moving wildfires, fanned by strong winds, are threatening homes and agricultural land. We are providing support to firefighting crews and offering assistance to families who have been evacuated from the area.", Status = "Active", FundingGoal = 75000, CurrentFunds = 25000, IncidentDate = DateTime.UtcNow  },
                     new DisasterIncident { Title = "Northern Cape Drought Intervention", Location = "Namaqualand, Northern Cape", ImageUrl = "https://i0.wp.com/www.dailymaverick.co.za/wp-content/uploads/2024/12/LHME0348.jpg?resize=600%2C342&quality=89&ssl=1", Description = "A multi-year drought has left farming communities and livestock in a critical condition. Our water provision teams are drilling boreholes and delivering millions of litres of water, along with animal feed to sustain local farmers.", Status = "Active", FundingGoal = 100000, CurrentFunds = 85000, IncidentDate = DateTime.UtcNow },
                     new DisasterIncident { Title = "Gauteng Flash Flood Assistance", Location = "Soweto, Gauteng", ImageUrl = "https://www.nsri.org.za/imager/banners/699096/stn-27-Gauteng-Flooding_82c5f5175a2e55ca0a26bbe33e7c08fd.jpg", Description = "A severe thunderstorm caused flash flooding in several low-lying urban areas, damaging hundreds of homes. We are distributing hygiene packs, mattresses, and food parcels to affected residents.", Status = "Active", FundingGoal = 40000, CurrentFunds = 15000, IncidentDate = DateTime.UtcNow },
                     new DisasterIncident { Title = "Free State Storm Rebuilding", Location = "Eastern Free State", ImageUrl = "https://centralnews.co.za/wp-content/uploads/2025/01/20250105_185830-1-1170x878.jpg", Description = "A severe hailstorm and tornado have caused extensive damage to infrastructure and homes in rural Free State towns. Our immediate focus is on providing building materials and volunteer support to help the community rebuild.", Status = "Active", FundingGoal = 120000, CurrentFunds = 30000, IncidentDate = DateTime.UtcNow },
                     new DisasterIncident { Title = "Eastern Cape Hunger Alleviation", Location = "Rural Eastern Cape", ImageUrl = "https://foodforwardsa.org/wp-content/uploads/2018/09/SAM_0209-1.jpg", Description = "We are running an ongoing hunger alleviation program in the rural Eastern Cape, providing monthly food parcels to vulnerable families, schools, and orphanages to ensure food security in the region.", Status = "Active", FundingGoal = 60000, CurrentFunds = 55000, IncidentDate = DateTime.UtcNow }
                 );
                 await context.SaveChangesAsync();
             }*/
        }
    }
}