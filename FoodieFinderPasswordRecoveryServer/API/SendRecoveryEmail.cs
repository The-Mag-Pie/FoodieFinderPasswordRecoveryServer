using FoodieFinderPasswordRecoveryServer.Database;
using FoodieFinderPasswordRecoveryServer.Models;
using MailKit;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using MimeKit;

namespace FoodieFinderPasswordRecoveryServer.API
{
    [Route("api/SendRecoveryEmail")]
    [ApiController]
    public class SendRecoveryEmail : ControllerBase
    {
        private const string EmailTemplate = @"Hello {0}!<br>Here is your password recovery link: https://{1}/NewPassword/{2}";

        private readonly AppDbContext _dbContext;
        private readonly SmtpData _smtpData;
        private readonly string _serverDomain;

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
            User userData;

            try
            {
                userData = _dbContext.User.Where(u => u.ID == userId).Single();
                if (userData.EncryptedPassword == "auth0")
                {
                    throw new();
                }
            }
            catch
            {
                return false.ToString();
            }

            var uuid = Guid.NewGuid().ToString();
            var epoch = DateTimeOffset.Now.ToUnixTimeSeconds();

            _dbContext.PasswordRecovery.Add(new()
            {
                UUID = uuid,
                CreatedEpoch = epoch,
                UserID = userId
            });

            _dbContext.SaveChanges();

            return _sendEmail(userData.Email, uuid).Result.ToString();
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

            using var client = new SmtpClient(new ProtocolLogger(Console.OpenStandardOutput()));
            client.Connect(_smtpData.Host, _smtpData.Port, true);
            client.Authenticate(_smtpData.Login, _smtpData.Password);
            client.MessageSent += (s, e) => sent = true;

            await client.SendAsync(message);
            client.Disconnect(true);

            return sent;
        }
    }
}
