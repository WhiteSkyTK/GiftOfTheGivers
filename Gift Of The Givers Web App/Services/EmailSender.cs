using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace Gift_Of_The_Givers_Web_App.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // This is a placeholder. In a real application, you would add logic
            // here to send an email using a service like SendGrid, Mailgun, or SMTP.
            return Task.CompletedTask;
        }
    }
}