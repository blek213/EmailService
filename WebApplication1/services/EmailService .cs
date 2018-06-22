using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string message, string callbackUrl)
        {
            try
            {
                using (var client = new SmtpClient())
                {
                    var credential = new NetworkCredential
                    {
                        UserName = "boxmelar@e-mail.ua",
                        Password = "passwordnotForreading123"
                    };

                    client.Credentials = credential;
                    //client.Host = _configuration["e-mail.ua"];
                    //client.Port = int.Parse(_configuration["587"]);
                    client.Host = "e-mail.ua";
                    client.Port = 587;
                    client.EnableSsl = true;
                   
                    using (var emailMessage = new MailMessage())
                    {
                        emailMessage.IsBodyHtml = true;
                        emailMessage.To.Add(new MailAddress(email));
                        //emailMessage.From = new MailAddress(_configuration["boxmelar@e-mail.ua"]);
                        emailMessage.From = new MailAddress("boxmelar@e-mail.ua");
                        emailMessage.Subject = subject;
                        emailMessage.Body = message + "<a href='" + callbackUrl + "'>" + "Confirm data" + "</a>";
                        client.Send(emailMessage);
                    }

                    await Task.CompletedTask;

                }
            }
            catch (Exception exc)
            {
                var a = exc;

            }
        }

        }
}
