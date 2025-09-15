using Gift_Of_The_Givers_Web_App.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Gift_Of_The_Givers_Web_App.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<DisasterIncident> DisasterIncidents { get; set; }
        public DbSet<ReliefProject> ReliefProjects { get; set; }
        public DbSet<Donation> Donations { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<ProjectVolunteer> ProjectVolunteers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProjectVolunteer>()
                .HasKey(pv => new { pv.ProjectID, pv.VolunteerUserID });
        }
    }
}
