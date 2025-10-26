using Gift_Of_The_Givers_Web_App.Models;
using Microsoft.AspNetCore.Mvc.Rendering; // Add this for SelectListItem

namespace Gift_Of_The_Givers_Web_App.ViewModels
{
    public class VolunteerHubViewModel
    {
        public List<DisasterIncident> MyAssignments { get; set; }
        public List<DisasterIncident> AvailableOpportunities { get; set; }

        // Add these properties for the filter
        public List<SelectListItem> Locations { get; set; }
        public string SelectedLocation { get; set; }
        public string SearchQuery { get; set; }
    }
}