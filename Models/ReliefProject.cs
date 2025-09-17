using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gift_Of_The_Givers_Web_App.Models
{
    public class ReliefProject
    {
        [Key]
        public int ProjectID { get; set; }

        [ForeignKey("DisasterIncident")]
        public int? IncidentID { get; set; } // Nullable

        [Required, StringLength(100)]
        public string ProjectName { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; } // Nullable

        [StringLength(50)]
        public string Status { get; set; }

        // Navigation property
        public virtual DisasterIncident DisasterIncident { get; set; }
        // Navigation property to hold all tasks for this project
        public virtual ICollection<VolunteerTask> Tasks { get; set; }
    }
}