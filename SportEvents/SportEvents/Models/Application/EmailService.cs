using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Text;
using System.Net;

namespace SportEvents.Models
{
    public class EmailService
    {
        private const string smtpUserName = "sportevents1@seznam.cz";
        private const string smtpPassword = "777003862";
        private const string smtpHost = "smtp.seznam.cz";
        private const int smtpPort = 25;


        public bool Send(string emailTo, string subject, string body)
        {
            try
            {
                using (var smtpClient = new SmtpClient())
                {
                    smtpClient.EnableSsl = false; //http
                    smtpClient.Host = smtpHost;
                    smtpClient.Port = smtpPort;
                    smtpClient.Timeout = 10000;
                    smtpClient.Credentials = new NetworkCredential(smtpUserName, smtpPassword);
                    var msg = new MailMessage
                    {
                        IsBodyHtml = true,
                        BodyEncoding = Encoding.UTF8,
                        From = new MailAddress(smtpUserName),
                        Subject = subject,
                        Body = body,
                        Priority = MailPriority.Normal,

                    };
                    msg.To.Add(emailTo);
                    smtpClient.SendMailAsync(msg);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}