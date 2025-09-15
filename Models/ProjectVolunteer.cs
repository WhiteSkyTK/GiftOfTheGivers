using System.ComponentModel.DataAnnotations.Schema;

namespace Gift_Of_The_Givers_Web_App.Models
{
    public class ProjectVolunteer
    {
        [ForeignKey("ReliefProject")]
        public int ProjectID { get; set; }

        [ForeignKey("User")]
        public int VolunteerUserID { get; set; }

        // Navigation properties
        public virtual ReliefProject ReliefProject { get; set; }
        public virtual User User { get; set; }
    }
}