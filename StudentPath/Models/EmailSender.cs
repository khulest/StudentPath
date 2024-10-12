using Microsoft.AspNetCore.Identity.UI.Services;

namespace StudentPath.Models
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            //Logic to send emails
           return Task.CompletedTask;
        }
    }
}
