using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Gift_Of_The_Givers_Web_App.Models
{
    // Inherit from the built-in IdentityUser
    public class ApplicationUser : IdentityUser
    {
        // Add your custom properties here
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        public DateTime DateRegistered { get; set; }

        public string VolunteerStatus { get; set; }

        public string? ProfilePictureUrl { get; set; }

    }
}