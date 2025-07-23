using System.Net;
using System.Net.Mail;

namespace ASP.Services.Email
{
    public class GmailService(IConfiguration configuration) : IEmailService
    {
        private readonly IConfiguration _configuration = configuration;
        public void Send(string to, string subject, string content)
        {
            var emailSection = _configuration.GetSection("Email") ?? throw new Exception("Configuration error: 'Email' section has not been found");
            var gmailSection = emailSection.GetSection("Gmail") ?? throw new Exception("Configuration error: 'Email.Gmail' section has not been found");

            string host = gmailSection.GetSection("Host")?.Value ?? throw new Exception("Configuration error: 'Email.Gmail.Host' section has not been found");
            int port = gmailSection.GetSection("Port")?.Get<int>() ?? throw new Exception("Configuration error: 'Email.Gmail.Port' section has not been found");
            string box = gmailSection.GetSection("Box")?.Value ?? throw new Exception("Configuration error: 'Email.Gmail.Box' section has not been found");
            string appKey = gmailSection.GetSection("AppKey")?.Value ?? throw new Exception("Configuration error: 'Email.Gmail.AppKey' section has not been found");

            using SmtpClient smtpClient = new(host) // using is for auto-dispose
            {
                Port = port,
                EnableSsl = true, // Secure channel
                Credentials = new NetworkCredential(box, appKey)
            };
            smtpClient.Send(box, to, subject, content);
        }
    }
}
