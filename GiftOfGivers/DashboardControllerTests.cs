using Gift_Of_The_Givers_Web_App.Controllers;
using Gift_Of_The_Givers_Web_App.Data;
using Gift_Of_The_Givers_Web_App.Models;
using Gift_Of_The_Givers_Web_App.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit; // Correct using for [Fact]
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ViewFeatures; // For TempData
using Microsoft.AspNetCore.Mvc.Controllers; // For ControllerActionDescriptor
using Microsoft.AspNetCore.Routing; // For RouteData

namespace Gift_Of_The_Givers_Web_App.UnitTests
{
    public class DashboardControllerTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbOptions;
        private Mock<UserManager<ApplicationUser>> _mockUserManager = null!; // Use null forgiving operator, will be initialized in constructor
        private ApplicationUser _testUser = null!; // Use null forgiving operator, will be initialized in constructor

        public DashboardControllerTests()
        {
            _dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"GofG_TestDb_{System.Guid.NewGuid()}")
                .Options;

            InitializeUserManagerMock(); // Call helper method
            InitializeTestUser();        // Call helper method
            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                            .ReturnsAsync(() => _testUser); // Use lambda to return current _testUser state
        }

        // Helper to initialize UserManager mock - keeps constructor clean
        private void InitializeUserManagerMock()
        {
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
        }

        // Helper to initialize the test user
        private void InitializeTestUser()
        {
            _testUser = new ApplicationUser
            {
                Id = "test-user-id-123",
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com", // Ensure Email is not null
                UserName = "test@example.com",
                DateRegistered = System.DateTime.UtcNow,
                VolunteerStatus = "Not Applied"
            };
        }

         private DashboardController CreateController(ApplicationDbContext context)
        {
             // Ensure the mock is set up (redundant if done in constructor, but safe)
            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                           .ReturnsAsync(_testUser);

            var controller = new DashboardController(context, _mockUserManager.Object);

            // Create ClaimsPrincipal
            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, _testUser.Id),
                new Claim(ClaimTypes.Name, _testUser.Email!)
            }, "mock"));

            // Setup HttpContext with the User
            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(ctx => ctx.User).Returns(userClaims);
             // Setup mock TempData
            var tempData = new TempDataDictionary(httpContextMock.Object, Mock.Of<ITempDataProvider>());


             // Assign ControllerContext
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContextMock.Object, // Use the mocked HttpContext
                RouteData = new RouteData(),
                ActionDescriptor = new ControllerActionDescriptor()
            };

            // Assign TempData directly to the controller
             controller.TempData = tempData;


            return controller;
        }

        [Fact]
        public void ReportIncident_GET_ReturnsViewResult()
        {
            using var context = new ApplicationDbContext(_dbOptions);
            var controller = CreateController(context); // This now sets up the user implicitly

            var result = controller.ReportIncident();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task ReportIncident_POST_ValidModel_CreatesIncidentAndRedirects()
        {
            var model = new ReportIncidentViewModel
            { /* Valid model */
                Title = "Test Flood",
                Location = "Test Location",
                Description = "Test Description"
            };

            await using (var context = new ApplicationDbContext(_dbOptions))
            {
                var controller = CreateController(context); // User setup is handled here

                // Act
                var result = await controller.ReportIncident(model);

                // Assert: Check Redirect and TempData
                var redirectResult = Assert.IsType<RedirectToActionResult>(result);
                Assert.Equal("Index", redirectResult.ActionName);
                Assert.Equal("Thank you! Your incident report has been submitted successfully.", controller.TempData["SuccessMessage"]);

                // Assert: Check Database
                var incidentInDb = await context.DisasterIncidents.FirstOrDefaultAsync();
                Assert.NotNull(incidentInDb);
                Assert.Equal(_testUser.Id, incidentInDb.ReportedByUserID); // This line *must* pass now
                Assert.Equal(model.Title, incidentInDb.Title);
                Assert.Equal("Pending Verification", incidentInDb.Status);
            }
        }

        [Fact]
        public async Task ReportIncident_POST_InvalidModel_ReturnsViewWithModel()
        {
            var model = new ReportIncidentViewModel { /* Invalid model */ };

            await using (var context = new ApplicationDbContext(_dbOptions))
            {
                var controller = CreateController(context); // User setup is handled here
                controller.ModelState.AddModelError("Title", "Required");

                // Act
                var result = await controller.ReportIncident(model);

                // Assert
                var viewResult = Assert.IsType<ViewResult>(result);
                Assert.False(viewResult.ViewData.ModelState.IsValid);
                Assert.Equal(model, viewResult.Model);
            }
        }

        // --- Add More Tests Here ---
        // Examples:
        // Test Index returns View
        // Test MyIncidents returns View with correct incidents for the user
        // Test ApplyToBeVolunteer POST updates user status and redirects
        // Test MakeDonation GET/POST (requires more complex setup/mocking if external services involved)

    }
}

