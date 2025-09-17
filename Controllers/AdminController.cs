using Gift_Of_The_Givers_Web_App.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
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

    public async Task<IActionResult> IncidentDetails(int incidentId)
    {
        var incident = await _context.DisasterIncidents
                                     .Include(i => i.ApplicationUser)
                                     .FirstOrDefaultAsync(i => i.IncidentID == incidentId);

        if (incident == null)
        {
            return NotFound();
        }

        return View(incident);
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
}