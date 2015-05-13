using System.IO;
using System.Web;
using System.Web.Hosting;
using SportEvents.Models;

namespace SportEvents
{
    public class HangFireCronTest
    {
        public void TestHangFire()
        {
            using (StreamWriter str = new StreamWriter(new FileStream(HttpContext.Current.Server.MapPath("~/Zapisovaci/TextFile1.txt"), FileMode.Append, FileAccess.Write)))
            {
                str.WriteLine("Test hangfire");
            }
        }

        public void TestEmailCron()
        {
            EmailService email = new EmailService();

            email.Send("dedykiller@seznam.cz", "Testujeme cron", "Ahoj, toto je test mail.");
        }
    }
}