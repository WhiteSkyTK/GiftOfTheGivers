using System.ComponentModel.DataAnnotations;

namespace Gift_Of_The_Givers_Web_App.ViewModels
{
    public class ReportIncidentViewModel
    {
        [Required]
        [Display(Name = "Incident Title")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Location (City, Province)")]
        public string Location { get; set; }

        [Required]
        [Display(Name = "Description of Incident")]
        public string Description { get; set; }

        [Display(Name = "Image URL (Optional)")]
        [Url]
        public string? ImageUrl { get; set; }

    }
}