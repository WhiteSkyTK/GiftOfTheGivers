using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GiftOfGiversUI
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class FunctionalUITests : PageTest
    {
        private const string BaseUrl = "https://localhost:7022/";

        public override BrowserNewContextOptions ContextOptions()
        {
            return new BrowserNewContextOptions()
            {
                BaseURL = BaseUrl,
            };
        }

        // --- Test 1: User Registration ---
        [Test]
        public async Task Register_New_User_Successfully()
        {
            var uniqueEmail = $"testuser_{Guid.NewGuid()}@example.com";

            // This password (Password123!) appears to be valid.
            var password = "Password123!";
            var firstName = "Test"; // Storing this to check the greeting

            await Page.GotoAsync("/Identity/Account/Register");
            await Page.Locator("#Input_FirstName").WaitForAsync(new() { State = WaitForSelectorState.Visible });
            await Page.Locator("#Input_FirstName").FillAsync(firstName);
            await Page.Locator("#Input_LastName").FillAsync("UserReg");
            await Page.Locator("#Input_Email").FillAsync(uniqueEmail);
            await Page.Locator("#Input_Password").FillAsync(password);
            await Page.Locator("#Input_ConfirmPassword").FillAsync(password);

            // --- FINAL FIX ---
            // We removed the try/catch. We now know registration SUCCEEDS
            // and redirects to the homepage.

            // This waits for *any* navigation to complete after the click.
            await Page.RunAndWaitForNavigationAsync(async () =>
            {
                await Page.Locator("#registerSubmit").ClickAsync();
            });

            // Now, we assert that we landed on the homepage
            await Expect(Page).ToHaveURLAsync(new Regex($"{BaseUrl}/?$"));

            // And we assert that we are logged in as the new user.
            // Note: We check for "Test" (with a capital T) because that's what we entered.
            await Expect(Page.Locator("nav .nav-link[title='Manage']")).ToContainTextAsync(firstName);
        }

        // --- Test 2: Login and Logout ---
        [Test]
        public async Task Login_And_Logout_Successfully()
        {
            // This user MUST exist in your database
            var existingUserEmail = "test@gmail.com";
            var existingUserPassword = "Password123!";

            await Page.GotoAsync("/Identity/Account/Login");
            await Page.Locator("#Input_Email").WaitForAsync(new() { State = WaitForSelectorState.Visible });
            await Page.Locator("#Input_Email").FillAsync(existingUserEmail);
            await Page.Locator("#Input_Password").FillAsync(existingUserPassword);

            // Combine click and wait
            await Page.RunAndWaitForNavigationAsync(async () =>
            {
                await Page.Locator("#login-submit").ClickAsync();
            }); // Wait for *any* redirect after login

            // Now that navigation is complete, wait for the element to appear
            await Page.Locator("nav .nav-link[title='Manage']").WaitForAsync(new() { State = WaitForSelectorState.Visible });

            // Check for the user's name ("test")
            await Expect(Page.Locator("nav .nav-link[title='Manage']")).ToContainTextAsync("test");

            // Combine click and wait for logout
            await Page.RunAndWaitForNavigationAsync(async () =>
            {
                await Page.Locator("nav form[action*='/Account/Logout'] button").ClickAsync();
            });

            await Expect(Page).ToHaveURLAsync(new Regex($"{BaseUrl}/?$"));
            await Expect(Page.Locator("nav a[href*='/Account/Login']")).ToBeVisibleAsync();
        }

        // --- Test 3: Donation Form (Monetary) ---
        [Test]
        public async Task Submit_Monetary_Donation_Successfully()
        {
            // This user MUST exist in your database
            var existingUserEmail = "test@gmail.com";
            var existingUserPassword = "Password123!";

            // Login
            await Page.GotoAsync("/Identity/Account/Login");
            await Page.Locator("#Input_Email").WaitForAsync(new() { State = WaitForSelectorState.Visible });
            await Page.Locator("#Input_Email").FillAsync(existingUserEmail);
            await Page.Locator("#Input_Password").FillAsync(existingUserPassword);

            // Combine click and wait for login
            await Page.RunAndWaitForNavigationAsync(async () =>
            {
                await Page.Locator("#login-submit").ClickAsync();
            });

            // Go to Donation Form
            await Page.GotoAsync("/Dashboard/MakeDonation");
            await Page.Locator("#Amount").WaitForAsync(new() { State = WaitForSelectorState.Visible });

            await Page.Locator("input[name='DonationType'][value='Monetary']").CheckAsync();
            await Page.Locator("#DisasterIncidentID").WaitForAsync(new() { State = WaitForSelectorState.Attached });

            // Select the second option in the dropdown (index 1)
            await Page.Locator("#DisasterIncidentID").SelectOptionAsync(new SelectOptionValue() { Index = 1 });

            await Page.Locator("#Amount").FillAsync("100");

            // Combine click and wait for form submission
            await Page.RunAndWaitForNavigationAsync(async () =>
            {
                await Page.Locator("form[action*='/Dashboard/MakeDonation'] button[type='submit']").ClickAsync();
            }, new() { UrlRegex = new Regex(".*/Dashboard$") }); // Wait for URL ending in /Dashboard

            // Now that navigation is done, check for the alert
            await Page.Locator(".alert.alert-success").WaitForAsync(new() { State = WaitForSelectorState.Visible });
            await Expect(Page.Locator(".alert.alert-success")).ToContainTextAsync("Thank you so much for your generous donation!");
            await Expect(Page).ToHaveURLAsync(new Regex(".*/Dashboard$"));
        }
    }
}

