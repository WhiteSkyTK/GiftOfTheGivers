using Gift_Of_The_Givers_Web_App.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Gift_Of_The_Givers_Web_App.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<DisasterIncident> DisasterIncidents { get; set; }
        public DbSet<Donation> Donations { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<ProjectVolunteer> ProjectVolunteers { get; set; }
        public DbSet<VolunteerTask> VolunteerTasks { get; set; }
        public DbSet<ResourceGoal> ResourceGoals { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }
        public DbSet<VolunteerAssignment> VolunteerAssignments { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<UserNotification> UserNotifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the composite primary key
            modelBuilder.Entity<ProjectVolunteer>()
                .HasKey(pv => new { pv.DisasterIncidentID, pv.VolunteerUserID });

            modelBuilder.Entity<VolunteerAssignment>()
                .HasKey(va => new { va.VolunteerUserID, va.TaskID });

            modelBuilder.Entity<ProjectVolunteer>()
                .HasOne(pv => pv.ApplicationUser)
                .WithMany()
                .HasForeignKey(pv => pv.VolunteerUserID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProjectVolunteer>()
                .HasOne(pv => pv.DisasterIncident)
                .WithMany()
                .HasForeignKey(pv => pv.DisasterIncidentID)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<DisasterIncident>()
                .HasOne(di => di.ApplicationUser)
                .WithMany()
                .HasForeignKey(di => di.ReportedByUserID)
                .OnDelete(DeleteBehavior.Restrict); 
                                                   
        }
    }
}
