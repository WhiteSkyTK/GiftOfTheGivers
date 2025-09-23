using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Gift_Of_The_Givers_Web_App.ViewModels
{
    public class SendMessageViewModel
    {
        [Required]
        public string RecipientType { get; set; } // "All", "Project", "Specific"

        public int? DisasterIncidentID { get; set; }
        public string? RecipientUserID { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        // For the dropdowns
        public List<SelectListItem> Incidents { get; set; } = new();
        public List<SelectListItem> Volunteers { get; set; } = new();
    }
}