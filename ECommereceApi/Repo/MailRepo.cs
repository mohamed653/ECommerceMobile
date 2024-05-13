using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;

namespace ECommereceApi.Repo
{
    public class MailRepo : IMailRepo
    {
        private readonly IConfiguration _configuration;
        public MailRepo(IConfiguration configuration)
        {
            _configuration = configuration;
        }



        public bool TrySendEmail(string email, string code)
        {
            string fromEmail = _configuration.GetValue<string>("fromEmail");

            fromEmail = "mohamedhamed3343@gmail.com";

            string toEmail = email;

            string? smtpServer = _configuration.GetValue<string>("Hostname");
            int port = _configuration.GetValue<int>("portnumber");

            string? password = _configuration.GetValue<string>("password");

           SmtpClient client = new SmtpClient(smtpServer, port)
           {
               Credentials = new NetworkCredential(fromEmail, password),
               EnableSsl = true 
           };

            MailMessage message = new MailMessage(fromEmail, toEmail)
            {
                Subject = "Email Verification",
                Body = $"{code}"
            };
            try
            {
                client.Send(message);
                return true;
            }
            catch
            {
                return false;
            }


        }
    }

}
