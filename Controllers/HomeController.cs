using Gift_Of_The_Givers_Web_App.Data;
using Gift_Of_The_Givers_Web_App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Gift_Of_The_Givers_Web_App.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public async Task<IActionResult> ActiveDisasters()
        {
            // Fetch all incidents from the database
            var incidents = await _context.DisasterIncidents
                                          .Where(i => i.Status == "Active") // Or another status you want to show
                                          .ToListAsync();
            return View(incidents);
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Donate()
        {
            var neededResources = await _context.Resources.ToListAsync();
            return View(neededResources);
        }

        // ADD THIS NEW ACTION to handle the form submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactMessage contactMessage)
        {
            if (ModelState.IsValid)
            {
                contactMessage.SubmissionDate = DateTime.UtcNow;
                _context.ContactMessages.Add(contactMessage);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Thank you for your message! We will get back to you shortly.";

                return RedirectToAction("Contact");
            }

            // If the form is invalid, show it again with the errors
            return View(contactMessage);
        }
    }
}
