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
        public async Task<IActionResult> Index()
        {
            // Run the check for overdue tasks first
            await UpdateOverdueTasks();
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

        [Authorize(Roles = "Admin,Volunteer")]
        public async Task<IActionResult> MyAssignments()
        {
            var userId = _userManager.GetUserId(User);
            var assignments = await _context.VolunteerAssignments
                .Where(va => va.VolunteerUserID == userId)
                .Include(va => va.VolunteerTask) // Load the task details
                    .ThenInclude(vt => vt.DisasterIncident) // And for each task, load the main incident details
                .OrderBy(va => va.VolunteerTask.TaskDate) // Order by the upcoming date
                .ToListAsync();
            return View(assignments);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Volunteer")]
        public async Task<IActionResult> SignUpForTask(int taskId)
        {
            var userId = _userManager.GetUserId(User);
            var isAlreadyAssigned = await _context.VolunteerAssignments
                .AnyAsync(va => va.TaskID == taskId && va.VolunteerUserID == userId);

            if (!isAlreadyAssigned)
            {
                var assignment = new VolunteerAssignment
                {
                    VolunteerUserID = userId,
                    TaskID = taskId,
                    Status = "Assigned"
                };
                _context.VolunteerAssignments.Add(assignment);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Successfully signed up for the task!";
            }
            else
            {
                TempData["ErrorMessage"] = "You are already signed up for this task.";
            }

            // Redirect back to the volunteer hub
            return RedirectToAction("MyAssignments");
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
        public async Task<IActionResult> ProjectDetails(int incidentId)
        {
            var project = await _context.DisasterIncidents
                                        .Include(p => p.Tasks)
                                        .Include(p => p.ResourceGoals) // THIS IS THE FIX: Include the goals
                                            .ThenInclude(rg => rg.Resource) // And for each goal, include the resource details
                                        .FirstOrDefaultAsync(p => p.IncidentID == incidentId);
            if (project == null) return NotFound();
            return View(project);
        }

        // GET action: Shows the donation form
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> MakeDonation(int? incidentId)
        {
            var viewModel = new MakeDonationViewModel
            {
                Incidents = await _context.DisasterIncidents
                    .Where(i => i.Status == "Active")
                    .Select(i => new SelectListItem { Text = i.Title, Value = i.IncidentID.ToString() })
                    .ToListAsync(),
                Resources = await _context.Resources
                    .Select(r => new SelectListItem { Text = r.ResourceName, Value = r.ResourceID.ToString() })
                    .ToListAsync(),
                DisasterIncidentID = incidentId // Pre-select the incident if an ID was passed
            };

            return View(viewModel);
        }

        // POST action: Processes the submitted donation
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakeDonation(MakeDonationViewModel model)
        {
            // Conditional validation logic based on DonationType
            if (model.DonationType == "Monetary")
            {
                if (!model.Amount.HasValue || model.Amount < 10) // Added minimum check
                {
                    ModelState.AddModelError(nameof(model.Amount), "Donation amount must be at least R10.");
                }
            }
            else if (model.DonationType == "Resource")
            {
                if (!model.ResourceID.HasValue)
                {
                    ModelState.AddModelError(nameof(model.ResourceID), "Please select an item to donate.");
                }
                if (!model.Quantity.HasValue || model.Quantity < 1) // Added minimum check
                {
                    ModelState.AddModelError(nameof(model.Quantity), "Quantity must be at least 1.");
                }
            }

            if (!ModelState.IsValid)
            {
                // Repopulate the dropdown lists before showing the form again
                model.Incidents = await _context.DisasterIncidents
                    .Where(i => i.Status == "Active")
                    .Select(i => new SelectListItem { Text = i.Title, Value = i.IncidentID.ToString() })
                    .ToListAsync();
                model.Resources = await _context.Resources
                    .Select(r => new SelectListItem { Text = r.ResourceName, Value = r.ResourceID.ToString() })
                    .ToListAsync();

                return View(model);
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var newDonation = new Donation
            {
                DonorUserID = currentUser.Id,
                DonationDate = DateTime.UtcNow,
                DisasterIncidentID = model.DisasterIncidentID,
                IsMonetary = model.DonationType == "Monetary",
                Amount = model.DonationType == "Monetary" ? model.Amount : null,
                ResourceID = model.DonationType == "Resource" ? model.ResourceID : null,
                Quantity = model.DonationType == "Resource" ? model.Quantity : null
            };

            _context.Donations.Add(newDonation);

            // If it's a monetary donation, update the incident's funds
            if (newDonation.IsMonetary && newDonation.DisasterIncidentID.HasValue)
            {
                var incident = await _context.DisasterIncidents.FindAsync(newDonation.DisasterIncidentID.Value);
                if (incident != null)
                {
                    incident.CurrentFunds += newDonation.Amount ?? 0;
                }
            }
            // --- ADD THIS LOGIC to handle resource donations ---
            else if (!newDonation.IsMonetary && newDonation.DisasterIncidentID.HasValue && newDonation.ResourceID.HasValue)
            {
                // Find the specific resource goal for this incident
                var resourceGoal = await _context.ResourceGoals.FirstOrDefaultAsync(
                    rg => rg.DisasterIncidentID == newDonation.DisasterIncidentID.Value &&
                          rg.ResourceID == newDonation.ResourceID.Value);

                // If a goal exists, update its current quantity
                if (resourceGoal != null)
                {
                    resourceGoal.CurrentQuantity += newDonation.Quantity ?? 0;
                }
            }
            // --- END OF ADDED LOGIC ---

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Thank you so much for your generous donation!";
            return RedirectToAction("Index"); // Redirect to the dashboard
        }

        [Authorize]
        public async Task<IActionResult> MyDonations()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            // This query finds donations where the DonorUserID matches the current user's ID
            var userDonations = await _context.Donations
                .Where(d => d.DonorUserID == currentUser.Id)
                .Include(d => d.DisasterIncident) // Include the project donated to
                .Include(d => d.Resource) // Include the item that was donated
                .OrderByDescending(d => d.DonationDate)
                .ToListAsync();

            return View(userDonations);
        }

        private async Task UpdateOverdueTasks()
        {
            var today = DateTime.UtcNow.Date;

            // Find all assignments that are still "Assigned" but whose task date is in the past
            var overdueAssignments = await _context.VolunteerAssignments
                .Include(va => va.VolunteerTask) // We need to include the task to check its date
                .Where(va => va.Status == "Assigned" && va.VolunteerTask.TaskDate < today)
                .ToListAsync();

            if (overdueAssignments.Any())
            {
                foreach (var assignment in overdueAssignments)
                {
                    assignment.Status = "Completed";
                }
                await _context.SaveChangesAsync();
            }
        }
    }
}
