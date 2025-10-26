using System.ComponentModel.DataAnnotations.Schema;

namespace Gift_Of_The_Givers_Web_App.Models
{
    public class VolunteerAssignment
    {
        [ForeignKey("ApplicationUser")]
        public string VolunteerUserID { get; set; }

        [ForeignKey("VolunteerTask")]
        public int TaskID { get; set; }

        public string Status { get; set; } // e.g., "Assigned", "Completed"

        // Navigation properties
        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual VolunteerTask VolunteerTask { get; set; }
    }
}
