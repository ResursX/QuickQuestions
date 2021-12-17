using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickQuestions.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfigurationRoot _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = (IConfigurationRoot)configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            IConfigurationSection emailConfiguration = _configuration.GetSection("Email");

            if(Convert.ToBoolean(emailConfiguration["Enabled"]))
            {
                MimeMessage emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress("Администрация QuickQuestions", emailConfiguration["Address"]));
                emailMessage.To.Add(new MailboxAddress("", email));
                emailMessage.Subject = subject;
                emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = htmlMessage
                };

                using (SmtpClient client = new SmtpClient())
                {
                    await client.ConnectAsync(emailConfiguration["Server"], Convert.ToInt32(emailConfiguration["Port"]), true);
                    await client.AuthenticateAsync(emailConfiguration["Address"], emailConfiguration["Password"]);

                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
                }
            }

            return;
        }
    }
}
