using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gift_Of_The_Givers_Web_App.Models
{
    public class VolunteerTask
    {
        [Key]
        public int TaskID { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        // Foreign key to link this task to a relief project
        [ForeignKey("DisasterIncident")]
        public int DisasterIncidentID { get; set; }

        public DateTime TaskDate { get; set; }

        // Navigation property
        public virtual ReliefProject ReliefProject { get; set; }
        // Navigation property
        public virtual DisasterIncident DisasterIncident { get; set; }
    }

}
