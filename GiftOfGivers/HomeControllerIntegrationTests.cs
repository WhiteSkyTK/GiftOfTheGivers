using Gift_Of_The_Givers_Web_App; // Needed for Program
using Gift_Of_The_Givers_Web_App.Data;
using Gift_Of_The_Givers_Web_App.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.TestHost; // Needed for ConfigureTestServices

// Adjust namespace if your project structure differs
namespace Gift_Of_The_Givers_Web_App.IntegrationTests
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                // 1. Remove the original ApplicationDbContext registration
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(ApplicationDbContext));
                if (dbContextDescriptor != null)
                {
                    services.Remove(dbContextDescriptor);
                }

                // 2. Remove the original DbContextOptions registration
                var dbContextOptionsDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (dbContextOptionsDescriptor != null)
                {
                    services.Remove(dbContextOptionsDescriptor);
                }

                // 3. Add DbContext with InMemory provider
                // Using AddDbContext is generally more stable for testing than AddDbContextPool.
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    // Use a unique name to ensure a fresh, isolated database for this test class.
                    options.UseInMemoryDatabase($"InMemoryDbForTesting_{Guid.NewGuid()}");
                });
            });

            builder.UseEnvironment("Development");
        }
    }

    public class HomeControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;

        public HomeControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task Get_Homepage_ReturnsSuccessAndCorrectContentType()
        {
            // Act
            var response = await _client.GetAsync("/");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType?.ToString());
        }

        /*[Fact]
        public async Task Get_ActiveDisasters_ReturnsOnlyActiveDisasters()
        {
            // Arrange: Seed the database
            // Create a scope from the factory's service provider
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<ApplicationDbContext>();
                var logger = scopedServices.GetRequiredService<ILogger<HomeControllerIntegrationTests>>();

                try
                {
                    logger.LogInformation("Attempting to seed database for Get_ActiveDisasters_ReturnsOnlyActiveDisasters test...");

                    // --- ADD THIS LINE BACK ---
                    // You MUST ensure the database schema is created before using it.
                    await db.Database.EnsureCreatedAsync();

                    // Clean data specifically for this test
                    db.DisasterIncidents.RemoveRange(db.DisasterIncidents);
                    await db.SaveChangesAsync();

                    // Add test data
                    db.DisasterIncidents.AddRange(
                        new DisasterIncident { Title = "Active Flood 1", Location = "Loc A", Status = "Active", IncidentDate = DateTime.UtcNow, FundingGoal = 1000, CurrentFunds = 100 },
                        new DisasterIncident { Title = "Pending Fire", Location = "Loc B", Status = "Pending Verification", IncidentDate = DateTime.UtcNow, FundingGoal = 2000, CurrentFunds = 0 },
                        new DisasterIncident { Title = "Active Drought", Location = "Loc C", Status = "Active", IncidentDate = DateTime.UtcNow, FundingGoal = 500, CurrentFunds = 500 }
                    );
                    await db.SaveChangesAsync();
                    logger.LogInformation("Database seeded successfully.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Database seeding failed.");
                    Assert.Fail($"Database seeding failed: {ex.Message} --- Inner Exception: {ex.InnerException?.Message}");
                }
            }

            // Act
            HttpResponseMessage response = null!;
            try
            {
                var logger = _factory.Services.GetRequiredService<ILogger<HomeControllerIntegrationTests>>();
                logger.LogInformation("Sending request to /Home/ActiveDisasters...");
                response = await _client.GetAsync("/Home/ActiveDisasters");
                logger.LogInformation("Received response with status code {StatusCode}", response.StatusCode);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                var errorContent = response != null ? await response.Content.ReadAsStringAsync() : "No response";
                Assert.Fail($"Request to /Home/ActiveDisasters failed. Status: {response?.StatusCode}. Content: {errorContent}. Exception: {ex.Message}");
            }

            // Assert
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Active Flood 1", content);
            Assert.Contains("Active Drought", content);
            Assert.DoesNotContain("Pending Fire", content);
        }*/
        // ... other integration tests ...
    }
}

