using System.ComponentModel.DataAnnotations.Schema;

namespace Gift_Of_The_Givers_Web_App.Models
{
    public class UserRole
    {
        [ForeignKey("User")]
        public int UserID { get; set; }

        [ForeignKey("Role")]
        public int RoleID { get; set; }

        // Navigation properties
        public virtual User User { get; set; }
        public virtual Role Role { get; set; }
    }
}