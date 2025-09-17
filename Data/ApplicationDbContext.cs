using Gift_Of_The_Givers_Web_App.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Gift_Of_The_Givers_Web_App.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<DisasterIncident> DisasterIncidents { get; set; }
        public DbSet<ReliefProject> ReliefProjects { get; set; }
        public DbSet<Donation> Donations { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<ProjectVolunteer> ProjectVolunteers { get; set; }
        public DbSet<VolunteerTask> VolunteerTasks { get; set; }
        public DbSet<ResourceGoal> ResourceGoals { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the composite primary key
            modelBuilder.Entity<ProjectVolunteer>()
                .HasKey(pv => new { pv.DisasterIncidentID, pv.VolunteerUserID });

            // --- ADD THIS BLOCK TO PREVENT CASCADE DELETE CYCLES ---
            modelBuilder.Entity<ProjectVolunteer>()
                .HasOne(pv => pv.ApplicationUser)
                .WithMany()
                .HasForeignKey(pv => pv.VolunteerUserID)
                .OnDelete(DeleteBehavior.Restrict); // Important

            modelBuilder.Entity<ProjectVolunteer>()
                .HasOne(pv => pv.DisasterIncident)
                .WithMany()
                .HasForeignKey(pv => pv.DisasterIncidentID)
                .OnDelete(DeleteBehavior.Restrict); // Important

            modelBuilder.Entity<DisasterIncident>()
                .HasOne(di => di.ApplicationUser)
                .WithMany()
                .HasForeignKey(di => di.ReportedByUserID)
                .OnDelete(DeleteBehavior.Restrict); // Important
                                                    // --- END OF BLOCK ---
        }
    }
}
