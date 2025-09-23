using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gift_Of_The_Givers_Web_App.Models
{
    public class UserNotification
    {
        [Key]
        public int UserNotificationID { get; set; }

        [Required]
        [ForeignKey("ApplicationUser")]
        public string UserID { get; set; } // The recipient volunteer's ID

        [Required]
        [ForeignKey("Notification")]
        public int NotificationID { get; set; } // The message being sent

        public bool IsRead { get; set; } = false;

        // Navigation properties
        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual Notification Notification { get; set; }
    }
}