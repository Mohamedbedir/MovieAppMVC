using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Tracing.DAL.Entities;

namespace WebAppTracingMvc.Helpers
{
    public class EmailSettings
    {
        // send Email 
        public static async Task SendEmailAsync(Email email)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(
                    "ozombedir15@gmail.com",
                    "aascsrmtlkzseyza"
                )
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(email.From),
                Subject = email.Subject,
                Body = email.Body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(email.To);

            await client.SendMailAsync(mailMessage);
        }
    }
}
