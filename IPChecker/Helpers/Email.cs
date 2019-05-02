using System;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace IPChecker
{
    public class Mailer
    {
        private readonly Settings secrets;
        private readonly string toAddress;
        private readonly string subject;
        private readonly string body;

        public Mailer(IConfiguration config, string toAddress, string subject, string body)
        {
            secrets = new Settings()
            {
                MailerEmail = config["Settings:MailerEmail"],
                MailerPassword = config["Settings:MailerPassword"]
            };
            this.toAddress = toAddress;
            this.subject = subject;
            this.body = body;
        }

        public bool Send()
        {
            try
            {
                var client = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential(secrets.MailerEmail, secrets.MailerPassword),
                    EnableSsl = true
                };

                MailMessage mail = new MailMessage("mwade290@gmail.com", toAddress, subject, body)
                {
                    IsBodyHtml = true
                };
                client.Send(mail);

                return true;
            }
            catch //(Exception ex)
            {
                //Log.Logger = new LoggerConfiguration()
                //    .WriteTo.File("webcrawler.log")
                //    .CreateLogger();

                //Log.Information(ex.ToString());
                //Log.Error(ex.Message);
                return false;
            }
        }
    }
}
