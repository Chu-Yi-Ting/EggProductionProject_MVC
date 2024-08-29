using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace EggProductionProject_MVC.Models
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var mail = new MailMessage();
            mail.From = new MailAddress("goodeggtworg@gmail.com");
            mail.To.Add(email);
            mail.Subject = subject;
            mail.IsBodyHtml = true;
            mail.Body = htmlMessage;

            SmtpClient client = new SmtpClient("smtp.gmail.com");
            //SmtpClient client = new SmtpClient("smtp.live.com");
            client.Port = 587;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential("goodeggtworg@gmail.com", "gzjn benu swzj omaw");
            client.EnableSsl = true;
            client.Send(mail);
        }
    }
}
