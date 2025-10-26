using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Gift_Of_The_Givers_Web_App.ViewModels
{
    public class MakeDonationViewModel
    {
        [Required]
        public string DonationType { get; set; } // "Monetary" or "Resource"

        [Display(Name = "Select a Project to Support")]
        public int? DisasterIncidentID { get; set; }

        [Display(Name = "Amount (R)")]
        [Range(10.0, double.MaxValue, ErrorMessage = "Donation amount must be at least R10.")]
        public decimal? Amount { get; set; }

        [Display(Name = "Select Item")]
        public int? ResourceID { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int? Quantity { get; set; }

        // Properties to hold the lists for the dropdowns
        public List<SelectListItem> Incidents { get; set; }
        public List<SelectListItem> Resources { get; set; }

        public MakeDonationViewModel()
        {
            Incidents = new List<SelectListItem>();
            Resources = new List<SelectListItem>();
        }
    }
}