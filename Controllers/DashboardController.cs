using Gift_Of_The_Givers_Web_App.Data; // For DbContext
using Gift_Of_The_Givers_Web_App.Models; // For your models
using Gift_Of_The_Givers_Web_App.ViewModels; // For the new ViewModel
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity; // For UserManager
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Gift_Of_The_Givers_Web_App.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Main dashboard page
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        // Page for the incident report form
        [HttpGet]
        [Authorize]
        public IActionResult ReportIncident()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReportIncident(ReportIncidentViewModel model)
        {
            // This 'if' statement is likely failing.
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);

                var incident = new DisasterIncident
                {
                    Title = model.Title,
                    Location = model.Location,
                    Description = model.Description,
                    ImageUrl = model.ImageUrl,
                    IncidentDate = DateTime.UtcNow,
                    Status = "Pending Verification",
                    ReportedByUserID = currentUser.Id,
                    FundingGoal = 10000.00m,
                    CurrentFunds = 0
                };

                _context.DisasterIncidents.Add(incident);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Thank you! Your incident report has been submitted successfully.";
                return RedirectToAction("Index");
            }

            // If ModelState is NOT valid, the code returns here, showing the form again.
            return View(model);
        }

        [Authorize(Roles = "Admin,Volunteer")]
        public async Task<IActionResult> VolunteerHub(string searchQuery, string selectedLocation)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var assignedIncidentIds = await _context.ProjectVolunteers
                .Where(pv => pv.VolunteerUserID == currentUser.Id)
                .Select(pv => pv.DisasterIncidentID)
                .ToListAsync();

            var myAssignments = await _context.DisasterIncidents
                .Where(i => assignedIncidentIds.Contains(i.IncidentID))
                .ToListAsync();

            // Start building the query for available opportunities
            var opportunitiesQuery = _context.DisasterIncidents
                .Where(i => i.Status == "Active" && !assignedIncidentIds.Contains(i.IncidentID));

            // --- FILTERING LOGIC ---
            if (!string.IsNullOrEmpty(searchQuery))
            {
                opportunitiesQuery = opportunitiesQuery.Where(i => i.Title.Contains(searchQuery) || i.Description.Contains(searchQuery));
            }

            if (!string.IsNullOrEmpty(selectedLocation))
            {
                opportunitiesQuery = opportunitiesQuery.Where(i => i.Location == selectedLocation);
            }

            var availableOpportunities = await opportunitiesQuery.ToListAsync();

            // --- POPULATE DROPDOWNS ---
            var locations = await _context.DisasterIncidents
                .Where(i => i.Status == "Active")
                .Select(i => i.Location)
                .Distinct()
                .Select(loc => new SelectListItem { Text = loc, Value = loc })
                .ToListAsync();

            var model = new VolunteerHubViewModel
            {
                MyAssignments = myAssignments,
                AvailableOpportunities = availableOpportunities,
                Locations = locations,
                SearchQuery = searchQuery,
                SelectedLocation = selectedLocation
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Volunteer")]
        public async Task<IActionResult> SignUpForIncident(int incidentId)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            // Check if already signed up to be safe
            var isAlreadySignedUp = await _context.ProjectVolunteers
                .AnyAsync(pv => pv.DisasterIncidentID == incidentId && pv.VolunteerUserID == currentUser.Id);

            if (!isAlreadySignedUp)
            {
                var newAssignment = new ProjectVolunteer
                {
                    DisasterIncidentID = incidentId,
                    VolunteerUserID = currentUser.Id
                };
                TempData["SuccessMessage"] = "Thank you! You have successfully signed up for this project.";
                _context.ProjectVolunteers.Add(newAssignment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("VolunteerHub");
        }

        [Authorize]
        public IActionResult MyProfile()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> MyIncidents()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var userIncidents = await _context.DisasterIncidents
                                              .Where(i => i.ReportedByUserID == currentUser.Id)
                                              .OrderByDescending(i => i.IncidentDate)
                                              .ToListAsync();
            return View(userIncidents);
        }

        [Authorize(Roles = "Admin,Volunteer")]
        public async Task<IActionResult> AssignmentDetails(int incidentId)
        {
            var incident = await _context.DisasterIncidents
                                         .FirstOrDefaultAsync(i => i.IncidentID == incidentId);
            if (incident == null)
            {
                return NotFound();
            }
            // We can create a more detailed ViewModel later
            return View(incident);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ApplyToBeVolunteer()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                user.VolunteerStatus = "Pending";
                await _userManager.UpdateAsync(user);
                TempData["SuccessMessage"] = "Your application to become a volunteer has been submitted for review.";
            }
            return RedirectToAction("VolunteerHub");
        }

        [Authorize] // Any logged-in user can see this page
        public IActionResult BecomeAVolunteer()
        {
            return View();
        }

        [Authorize]
        public IActionResult VolunteerApplication()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SubmitVolunteerApplication()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                user.VolunteerStatus = "Pending";
                await _userManager.UpdateAsync(user);
                TempData["SuccessMessage"] = "Your application to become a volunteer has been submitted for review.";
            }
            return RedirectToAction("VolunteerApplication");
        }

        [Authorize(Roles = "Admin,Volunteer")]
        public async Task<IActionResult> ProjectDetails(int incidentId) // Assuming projects are linked to incidents
        {
            // This logic will need to be expanded to get a ReliefProject and its tasks
            var project = await _context.DisasterIncidents
                                        .Include(p => p.Tasks) // You'll need to include the tasks
                                        .FirstOrDefaultAsync(p => p.IncidentID == incidentId);
            if (project == null)
            {
                return NotFound();
            }
            return View(project);
        }
    }
}
