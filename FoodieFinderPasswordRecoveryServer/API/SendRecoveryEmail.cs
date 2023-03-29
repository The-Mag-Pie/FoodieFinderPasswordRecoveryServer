using FoodieFinderPasswordRecoveryServer.Database;
using FoodieFinderPasswordRecoveryServer.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using MimeKit;

namespace FoodieFinderPasswordRecoveryServer.API
{
    [Route("api/SendRecoveryEmail")]
    [ApiController]
    public class SendRecoveryEmail : ControllerBase
    {
        private const string EmailTemplate = @"Hello {0}!<br>Here is your password recovery link: https://{1}/PasswordRecovery/NewPassword/{2}";

        private AppDbContext _dbContext;
        private SmtpData _smtpData;
        private string _serverDomain;

        public SendRecoveryEmail(AppDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _smtpData = configuration.GetSection("SmtpData").Get<SmtpData>() 
                ?? throw new ArgumentNullException(nameof(_smtpData));
            _serverDomain = configuration.GetValue<string>("ServerDomain")
                ?? throw new ArgumentNullException(nameof(_serverDomain));
        }

        [HttpGet("{userId}")]
        public string Get(int userId)
        {
            return _sendEmail("bartek.sroka@op.pl", "hshshshhs").Result.ToString();
        }

        private async Task<bool> _sendEmail(string email, string uuid)
        {
            bool sent = false;

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("FoodieFinder", _smtpData.Email));
            message.To.Add(new MailboxAddress(email, email));
            message.Subject = "FoodieFinder Password Recovery";
            message.Body = new TextPart("html")
            {
                Text = string.Format(EmailTemplate, email, _serverDomain, uuid)
            };

            using var client = new SmtpClient();
            client.Connect(_smtpData.Host, _smtpData.Port, false);
            //client.AuthenticationMechanisms.Clear();
            client.Authenticate(_smtpData.Login, _smtpData.Password);
            client.MessageSent += (s, e) => sent = true;

            await client.SendAsync(message);
            client.Disconnect(true);

            return sent;

            //bool sent = false;

            //using var smtpClient = new SmtpClient(_smtpData.Host, 465);
            //smtpClient.Credentials = new NetworkCredential(_smtpData.Login, _smtpData.Password);
            //smtpClient.EnableSsl = true;

            //var from = new MailAddress(_smtpData.Email, "FoodieFinder");
            //var to = new MailAddress(email);

            //using var message = new MailMessage(from, to);
            //message.Body = string.Format(EmailTemplate, email, _serverDomain, uuid);
            //message.BodyEncoding = Encoding.UTF8;
            //message.Subject = "FoodieFinder Password Recovery";
            //message.SubjectEncoding = Encoding.UTF8;

            //smtpClient.SendCompleted += (s, e) => sent = true;

            //smtpClient.Send(message);

            //return sent;
        }
    }
}
