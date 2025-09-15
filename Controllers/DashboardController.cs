using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gift_Of_The_Givers_Web_App.Controllers
{
    [Authorize] // This protects the entire controller
    public class DashboardController : Controller
    {
        // Main dashboard page
        public IActionResult Index()
        {
            return View();
        }

        // Page for the incident report form
        public IActionResult ReportIncident()
        {
            return View();
        }

        // Page for the volunteer hub
        public IActionResult VolunteerHub()
        {
            return View();
        }

        public IActionResult MyProfile()
        {
            return View();
        }
    }
}
