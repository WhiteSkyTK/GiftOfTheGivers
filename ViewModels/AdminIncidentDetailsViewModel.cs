using Gift_Of_The_Givers_Web_App.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Gift_Of_The_Givers_Web_App.ViewModels
{
    public class AdminIncidentDetailsViewModel
    {
        [ValidateNever]
        public DisasterIncident Incident { get; set; }
        public VolunteerTask NewTask { get; set; }
    }
}