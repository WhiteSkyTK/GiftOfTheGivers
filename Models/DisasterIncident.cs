using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gift_Of_The_Givers_Web_App.Models
{
    public class DisasterIncident
    {
        [Key]
        public int IncidentID { get; set; }

        [Required, StringLength(100)]
        public string Title { get; set; }

        [ForeignKey("ApplicationUser")]
        public string? ReportedByUserID { get; set; }

        [Required, StringLength(255)]
        public string Location { get; set; } = string.Empty;

        [Column(TypeName = "decimal(9, 6)")]
        public decimal Latitude { get; set; }

        [Column(TypeName = "decimal(9, 6)")]
        public decimal Longitude { get; set; }

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal FundingGoal { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal CurrentFunds { get; set; }

        public DateTime IncidentDate { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = string.Empty;

        // Navigation property
        public virtual ApplicationUser? ApplicationUser { get; set; }
        // Navigation property to hold all tasks for this incident
        public virtual ICollection<VolunteerTask> Tasks { get; set; }
    }
}