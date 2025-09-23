using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gift_Of_The_Givers_Web_App.Models
{
    public class Notification
    {
        [Key]
        public int NotificationID { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; } // The body of the message

        public DateTime Timestamp { get; set; }

        // Optional: Link to a project for project-specific messages
        public int? DisasterIncidentID { get; set; }
        public virtual DisasterIncident? DisasterIncident { get; set; }
    }
}