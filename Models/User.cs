using System.ComponentModel.DataAnnotations;

namespace Gift_Of_The_Givers_Web_App.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }
        [Required, StringLength(50)]
        public string FirstName { get; set; }
        [Required, StringLength(50)]
        public string LastName { get; set; }
        [Required, EmailAddress, StringLength(100)]
        public string Email { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        [Phone, StringLength(20)]
        public string? PhoneNumber { get; set; }
        public DateTime DateRegistered { get; set; }
    }
}
