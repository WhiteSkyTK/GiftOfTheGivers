using Gift_Of_The_Givers_Web_App.Controllers;
using Gift_Of_The_Givers_Web_App.Data;
using Gift_Of_The_Givers_Web_App.Models;
using Gift_Of_The_Givers_Web_App.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.ViewFeatures; // Needed for TempData


namespace Gift_Of_The_Givers_Web_App.UnitTests
{
    public class AdminControllerTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbOptions;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly ApplicationUser _testAdminUser;
        private readonly ApplicationUser _testVolunteerUser;

        public AdminControllerTests()
        {
            _dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"GofG_AdminTestDb_{Guid.NewGuid()}")
                .Options;

            // --- Mock UserManager Setup (Same as DashboardControllerTests) ---
            var store = new Mock<IUserStore<ApplicationUser>>();
            var options = new Mock<IOptions<IdentityOptions>>();
            var identityOptions = new IdentityOptions();
            options.Setup(o => o.Value).Returns(identityOptions);
            var userValidators = new List<IUserValidator<ApplicationUser>>();
            var passwordValidators = new List<IPasswordValidator<ApplicationUser>>();

            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                store.Object, options.Object, new Mock<IPasswordHasher<ApplicationUser>>().Object,
                userValidators, passwordValidators, new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object, new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<ApplicationUser>>>().Object);
            // --- End Mock UserManager Setup ---

            _testAdminUser = new ApplicationUser { Id = "admin-id", UserName = "admin@example.com", Email = "admin@example.com", FirstName = "Admin", LastName = "User" };
            _testVolunteerUser = new ApplicationUser { Id = "volunteer-id", UserName = "volunteer@example.com", Email = "volunteer@example.com", FirstName = "Volunteer", LastName = "Applicant", VolunteerStatus = "Pending" };

            // Setup mock responses
            _mockUserManager.Setup(um => um.FindByIdAsync("volunteer-id")).ReturnsAsync(_testVolunteerUser);
            _mockUserManager.Setup(um => um.RemoveFromRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(um => um.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);

            // Seed initial data if needed for specific tests
            SeedDatabase();
        }

        private void SeedDatabase()
        {
            using var context = new ApplicationDbContext(_dbOptions);
            // Example: Add an incident if needed for AddTaskToIncident tests
            if (!context.DisasterIncidents.Any())
            {
                context.DisasterIncidents.Add(new DisasterIncident
                {
                    IncidentID = 1,
                    Title = "Initial Test Incident",
                    Location = "Test Location",
                    Status = "Active",
                    IncidentDate = DateTime.UtcNow.AddDays(-1)
                });
                context.SaveChanges();
            }
        }

        private AdminController CreateController(ApplicationDbContext context)
        {
            var controller = new AdminController(context, _mockUserManager.Object);

            // Mock TempData - needed for AddTaskToIncident tests
            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            controller.TempData = tempData;

            // Mock User claims if controller actions depend on User.Identity
            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                 new Claim(ClaimTypes.NameIdentifier, _testAdminUser.Id), // Simulate admin being logged in
                 new Claim(ClaimTypes.Name, _testAdminUser.Email!)
            }, "mock"));
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userClaims }
            };


            return controller;
        }

        [Fact]
        public async Task ApproveVolunteer_ValidUserId_UpdatesUserAndRedirects()
        {
            // Arrange
            await using var context = new ApplicationDbContext(_dbOptions); // Context not strictly needed here but good practice
            var controller = CreateController(context);

            // Act
            var result = await controller.ApproveVolunteer("volunteer-id");

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ManageVolunteers", redirectResult.ActionName);

            // Verify UserManager calls
            _mockUserManager.Verify(um => um.FindByIdAsync("volunteer-id"), Times.Once);
            _mockUserManager.Verify(um => um.RemoveFromRoleAsync(_testVolunteerUser, "GeneralUser"), Times.Once); // Make sure role names match your SeedData
            _mockUserManager.Verify(um => um.AddToRoleAsync(_testVolunteerUser, "Volunteer"), Times.Once);
            _mockUserManager.Verify(um => um.UpdateAsync(It.Is<ApplicationUser>(u => u.Id == "volunteer-id" && u.VolunteerStatus == "Approved")), Times.Once);
        }

        [Fact]
        public async Task ApproveVolunteer_InvalidUserId_RedirectsWithoutChanges()
        {
            // Arrange
            _mockUserManager.Setup(um => um.FindByIdAsync("invalid-id")).ReturnsAsync((ApplicationUser?)null); // Setup for invalid user
            await using var context = new ApplicationDbContext(_dbOptions);
            var controller = CreateController(context);


            // Act
            var result = await controller.ApproveVolunteer("invalid-id");

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ManageVolunteers", redirectResult.ActionName);

            // Verify UserManager calls (UpdateAsync should NOT be called)
            _mockUserManager.Verify(um => um.FindByIdAsync("invalid-id"), Times.Once);
            _mockUserManager.Verify(um => um.UpdateAsync(It.IsAny<ApplicationUser>()), Times.Never); // Ensure user wasn't updated
            _mockUserManager.Verify(um => um.RemoveFromRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
            _mockUserManager.Verify(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task AddTaskToIncident_ValidModel_AddsTaskAndRedirects()
        {
            // Arrange
            var newTask = new VolunteerTask
            {
                DisasterIncidentID = 1, // Matches seeded incident
                Title = "New Test Task",
                Description = "Description for new task",
                TaskDate = DateTime.UtcNow.AddDays(5)
            };
            var viewModel = new AdminIncidentDetailsViewModel { NewTask = newTask };

            await using (var context = new ApplicationDbContext(_dbOptions))
            {
                var controller = CreateController(context);

                // Act
                var result = await controller.AddTaskToIncident(viewModel);

                // Assert
                var redirectResult = Assert.IsType<RedirectToActionResult>(result);
                Assert.Equal("IncidentDetails", redirectResult.ActionName);
                Assert.Equal(1, redirectResult.RouteValues?["incidentId"]);
                Assert.Equal("Task added successfully!", controller.TempData["SuccessMessage"]); // Check TempData

                // Verify task was added to the database
                var taskInDb = await context.VolunteerTasks.FirstOrDefaultAsync(t => t.Title == "New Test Task");
                Assert.NotNull(taskInDb);
                Assert.Equal(1, taskInDb.DisasterIncidentID);
            }
        }

        [Fact]
        public async Task AddTaskToIncident_InvalidModel_ReturnsViewWithError()
        {
            // Arrange
            var newTask = new VolunteerTask { DisasterIncidentID = 1 }; // Missing Title, Description, Date
            var viewModel = new AdminIncidentDetailsViewModel { NewTask = newTask };
            // Add initial incident required by the logic when returning the view
            var initialIncident = new DisasterIncident { IncidentID = 1, Title = "Initial Incident" };
            viewModel.Incident = initialIncident; // Add this line


            await using (var context = new ApplicationDbContext(_dbOptions))
            {
                var controller = CreateController(context);
                controller.ModelState.AddModelError("NewTask.Title", "The Title field is required."); // Simulate validation error

                // Act
                var result = await controller.AddTaskToIncident(viewModel);

                // Assert
                var viewResult = Assert.IsType<ViewResult>(result);
                Assert.Equal("IncidentDetails", viewResult.ViewName); // Check it returns the correct view
                Assert.False(controller.ModelState.IsValid);
                Assert.True(controller.ModelState.ContainsKey("NewTask.Title"));
                Assert.Equal("Failed to add task. Please check the form for errors.", controller.TempData["ErrorMessage"]); // Check TempData

                // Verify task was NOT added
                var taskInDb = await context.VolunteerTasks.FirstOrDefaultAsync(t => t.Title == "New Test Task"); // Use a title that wasn't added
                Assert.Null(taskInDb);
            }
        }

        // --- Add More Tests Here ---
        // Examples:
        // Test ManageIncidents GET
        // Test IncidentDetails GET (valid and invalid ID)
        // Test ApproveIncident POST
        // Test EditIncident GET/POST
        // Test AddResourceGoal POST (valid and invalid)
        // Test DeleteIncident GET/POST
        // Test DonationHistory GET
        // Test CommunicationCenter GET
        // Test SendMessage POST (for different recipient types)
    }
}
