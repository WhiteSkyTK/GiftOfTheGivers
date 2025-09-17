using Gift_Of_The_Givers_Web_App.Data;
using Gift_Of_The_Givers_Web_App.Models;
using Gift_Of_The_Givers_Web_App.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> ManageIncidents()
    {
        // Update this query to include the user who reported it
        var allIncidents = await _context.DisasterIncidents
                                         .Include(i => i.ApplicationUser)
                                         .ToListAsync();
        return View(allIncidents);
    }

    //[HttpGet] IncidentDetails action
    public async Task<IActionResult> IncidentDetails(int incidentId)
    {
        var incident = await _context.DisasterIncidents
                                     .Include(i => i.Tasks)
                                     .FirstOrDefaultAsync(i => i.IncidentID == incidentId);
        if (incident == null)
        {
            return NotFound();
        }

        var model = new AdminIncidentDetailsViewModel
        {
            Incident = incident,
            NewTask = new VolunteerTask { DisasterIncidentID = incidentId, TaskDate = DateTime.Today }
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddTaskToIncident(AdminIncidentDetailsViewModel model)
    {
        // We only need to check if the NewTask part of the model is valid
        if (ModelState.IsValid)
        {
            // Add the new task to the context
            _context.VolunteerTasks.Add(model.NewTask);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Task added successfully!";
            return RedirectToAction("IncidentDetails", new { incidentId = model.NewTask.DisasterIncidentID });
        }

        // --- If validation fails, do this instead of redirecting ---
        // We must reload the main incident data before showing the page again
        model.Incident = await _context.DisasterIncidents
                                       .Include(i => i.Tasks)
                                       .FirstOrDefaultAsync(i => i.IncidentID == model.NewTask.DisasterIncidentID);

        // Return to the same view, which will now show validation errors
        return View("IncidentDetails", model);
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
        var incident = await _context.DisasterIncidents.FindAsync(incidentId);
        if (incident == null)
        {
            return NotFound();
        }

        // Create a ViewModel from the database model
        var viewModel = new EditIncidentViewModel
        {
            IncidentID = incident.IncidentID,
            ReportedByUserID = incident.ReportedByUserID,
            Title = incident.Title,
            Location = incident.Location,
            Description = incident.Description,
            ImageUrl = incident.ImageUrl,
            FundingGoal = incident.FundingGoal,
            CurrentFunds = incident.CurrentFunds,
            Status = incident.Status
        };

        return View(viewModel);
    }

    // GET action for the Edit page
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditIncident(EditIncidentViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            // 1. Load the original incident from the database
            var incidentToUpdate = await _context.DisasterIncidents.FindAsync(viewModel.IncidentID);

            if (incidentToUpdate == null)
            {
                return NotFound();
            }

            // 2. Update its properties with the values from the form
            incidentToUpdate.Title = viewModel.Title;
            incidentToUpdate.Location = viewModel.Location;
            incidentToUpdate.Description = viewModel.Description;
            incidentToUpdate.ImageUrl = viewModel.ImageUrl;
            incidentToUpdate.FundingGoal = viewModel.FundingGoal;
            incidentToUpdate.CurrentFunds = viewModel.CurrentFunds;
            incidentToUpdate.Status = viewModel.Status;

            // 3. Save the changes
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Incident updated successfully!";
            return RedirectToAction("ManageIncidents");
        }

        // If the model is not valid, return to the view with the current data to show errors
        return View(viewModel);
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
}