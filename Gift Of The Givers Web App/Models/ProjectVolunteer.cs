using System.ComponentModel.DataAnnotations.Schema;

namespace Gift_Of_The_Givers_Web_App.Models
{
    public class ProjectVolunteer
    {
        [ForeignKey("DisasterIncident")]
        public int DisasterIncidentID { get; set; }

        [ForeignKey("ApplicationUser")]
        public string VolunteerUserID { get; set; }

        // Navigation properties
        public virtual DisasterIncident DisasterIncident { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}