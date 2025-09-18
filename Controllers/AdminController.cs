using Gift_Of_The_Givers_Web_App.Data;
using Gift_Of_The_Givers_Web_App.Models;
using Gift_Of_The_Givers_Web_App.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        // Run the check for overdue tasks first
        await UpdateOverdueTasks();
        return View();
    }

    private async Task UpdateOverdueTasks()
    {
        var today = DateTime.UtcNow.Date;

        // Find all assignments that are still "Assigned" but whose task date is in the past
        var overdueAssignments = await _context.VolunteerAssignments
            .Include(va => va.VolunteerTask)
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

    public async Task<IActionResult> ManageIncidents()
    {
        // Update this query to include the user who reported it
        var allIncidents = await _context.DisasterIncidents
                                         .Include(i => i.ApplicationUser)
                                         .ToListAsync();
        return View(allIncidents);
    }

    public async Task<IActionResult> IncidentDetails(int incidentId)
    {
        var incident = await _context.DisasterIncidents
                                     .Include(i => i.Tasks)
                                     .Include(i => i.ApplicationUser)
                                     .FirstOrDefaultAsync(i => i.IncidentID == incidentId);
        if (incident == null)
        {
            return NotFound();
        }

        // CREATE THE VIEWMODEL and pass it to the view
        var viewModel = new AdminIncidentDetailsViewModel
        {
            Incident = incident,
            NewTask = new VolunteerTask { DisasterIncidentID = incidentId, TaskDate = DateTime.Today }
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddTaskToIncident(AdminIncidentDetailsViewModel model)
    {
        // THIS IS THE FIX for when validation fails
        // We must check the validation state of the NewTask property specifically
        if (!ModelState.IsValid)
        {
            // We must reload the main incident data before showing the page again
            model.Incident = await _context.DisasterIncidents
                                           .Include(i => i.Tasks)
                                           .FirstOrDefaultAsync(i => i.IncidentID == model.NewTask.DisasterIncidentID);

            TempData["ErrorMessage"] = "Failed to add task. Please check the form for errors.";
            return View("IncidentDetails", model);
        }

        _context.VolunteerTasks.Add(model.NewTask);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Task added successfully!";
        return RedirectToAction("IncidentDetails", new { incidentId = model.NewTask.DisasterIncidentID });
    }

    [HttpPost]
    public async Task<IActionResult> ApproveIncident(int incidentId)
    {
        var incident = await _context.DisasterIncidents.FindAsync(incidentId);
        if (incident != null)
        {
            incident.Status = "Active";
            await _context.SaveChangesAsync();
        }
        return RedirectToAction("ManageIncidents");
    }

    public async Task<IActionResult> ManageVolunteers()
    {
        var pendingApplicants = await _userManager.Users
            .Where(u => u.VolunteerStatus == "Pending")
            .ToListAsync();
        return View(pendingApplicants);
    }

    // ADD THIS ACTION
    [HttpPost]
    public async Task<IActionResult> ApproveVolunteer(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            // Change status and update roles
            user.VolunteerStatus = "Approved";
            await _userManager.RemoveFromRoleAsync(user, "GeneralUser");
            await _userManager.AddToRoleAsync(user, "Volunteer");
            await _userManager.UpdateAsync(user);
        }
        return RedirectToAction("ManageVolunteers");
    }

    [HttpGet]
    public async Task<IActionResult> EditIncident(int incidentId)
    {
        var incident = await _context.DisasterIncidents
            .Include(i => i.ResourceGoals).ThenInclude(rg => rg.Resource)
            .FirstOrDefaultAsync(i => i.IncidentID == incidentId);

        if (incident == null) return NotFound();

        // This ViewBag is essential for the "Add Resource Goal" dropdown
        ViewBag.Resources = await _context.Resources
                                          .Select(r => new SelectListItem { Text = r.ResourceName, Value = r.ResourceID.ToString() })
                                          .ToListAsync();
        return View(incident);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditIncident(DisasterIncident incident)
    {
        if (ModelState.IsValid)
        {
            // --- This is a safer way to update to prevent losing data ---
            var incidentFromDb = await _context.DisasterIncidents.FindAsync(incident.IncidentID);
            if (incidentFromDb == null)
            {
                return NotFound();
            }

            // Map the updated values from the form to the existing incident
            incidentFromDb.Title = incident.Title;
            incidentFromDb.Location = incident.Location;
            incidentFromDb.Description = incident.Description;
            incidentFromDb.FundingGoal = incident.FundingGoal;
            incidentFromDb.Status = incident.Status;
            incidentFromDb.MeetingPoint = incident.MeetingPoint;
            incidentFromDb.OnSiteContact = incident.OnSiteContact;

            _context.Update(incidentFromDb);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Incident updated successfully!";
            return RedirectToAction("ManageIncidents");
        }

        // --- If validation fails, we MUST reload the ViewBag data ---
        ViewBag.Resources = await _context.Resources
            .Select(r => new SelectListItem { Text = r.ResourceName, Value = r.ResourceID.ToString() })
            .ToListAsync();

        return View(incident);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddResourceGoal(ResourceGoal resourceGoal)
    {
        // First, check if the goal quantity is valid.
        if (ModelState.IsValid)
        {
            // If it is, save the new goal and redirect back to the page
            _context.ResourceGoals.Add(resourceGoal);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Resource goal added successfully!";
            return RedirectToAction("EditIncident", new { incidentId = resourceGoal.DisasterIncidentID });
        }

        // --- THIS IS THE FIX ---
        // If we get here, validation failed. We must NOT redirect.
        TempData["ErrorMessage"] = "Could not add resource goal. Please check the quantity.";

        // We need to reload all the data for the EditIncident page to display it again.
        var incident = await _context.DisasterIncidents
            .Include(i => i.ResourceGoals).ThenInclude(rg => rg.Resource)
            .FirstOrDefaultAsync(i => i.IncidentID == resourceGoal.DisasterIncidentID);

        if (incident == null)
        {
            return NotFound(); // Safety check
        }

        // We must also repopulate the ViewBag for the dropdown list.
        ViewBag.Resources = await _context.Resources
            .Select(r => new SelectListItem { Text = r.ResourceName, Value = r.ResourceID.ToString() })
            .ToListAsync();

        // Now, return the EditIncident view. The validation errors will now be displayed.
        return View("EditIncident", incident);
    }

    // POST action to delete an incident
    [HttpGet]
    public async Task<IActionResult> DeleteIncident(int incidentId)
    {
        var incident = await _context.DisasterIncidents
                                     .FirstOrDefaultAsync(i => i.IncidentID == incidentId);
        if (incident == null)
        {
            return NotFound();
        }
        return View(incident);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteIncidentConfirmed(int incidentId) // Renamed for clarity
    {
        var incident = await _context.DisasterIncidents.FindAsync(incidentId);
        if (incident != null)
        {
            _context.DisasterIncidents.Remove(incident);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction("ManageIncidents");
    }

    public async Task<IActionResult> DonationHistory()
    {
        var allDonations = await _context.Donations
            .Include(d => d.ApplicationUser) // Include the user who donated
            .Include(d => d.DisasterIncident) // Include the project donated to
            .Include(d => d.Resource) // Include the item that was donated
            .OrderByDescending(d => d.DonationDate)
            .ToListAsync();

        return View(allDonations);
    }
}