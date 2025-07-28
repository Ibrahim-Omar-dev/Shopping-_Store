using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Utility
{
    public class EmailSender : IEmailSender
    {
        private readonly string _apiKey;

        public EmailSender(IConfiguration  configuration)
        {
            _apiKey = configuration["SendGrid:ApiKey"];
        }
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress("hemaomar1qaz2wsx@gmail.com", "Bullky APP");
            var to = new EmailAddress(email);
            var message = MailHelper.CreateSingleEmail(from, to, subject, "", htmlMessage);
            return client.SendEmailAsync(message);
        }
    }
}
