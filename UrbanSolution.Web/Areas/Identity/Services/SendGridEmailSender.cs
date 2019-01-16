using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace UrbanSolution.Web.Areas.Identity.Services
{
    public class SendGridEmailSender : IEmailSender
    {
        private readonly IConfiguration configuration;

        public SendGridEmailSender(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var apiKey = this.configuration.GetSection("SendGrid:APIKey").Value;

            var client = new SendGridClient(apiKey);

            var from = new EmailAddress("admin@urbansolution.bg", "UrbanSolution Admin");

            var to = new EmailAddress(email, email);

            var msg = MailHelper.CreateSingleEmail(from, to, subject, htmlMessage, htmlMessage);

            var response = await client.SendEmailAsync(msg);

            var body = await response.Body.ReadAsStringAsync();

            var statusCode = response.StatusCode;
        }
    }
}
