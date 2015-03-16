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
        public bool Send(string smtpUserName, string smtpPassword, string smtpHost, int smtpPort, string emailTo, string subject, string body)
        {
            try
            {
                using (var smtpClient = new SmtpClient())
                {
                    smtpClient.EnableSsl = false; //http
                    smtpClient.Host = smtpHost;
                    smtpClient.Port = smtpPort;
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
                    smtpClient.Send(msg);
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