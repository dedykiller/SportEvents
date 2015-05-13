using System.IO;
using System.Web;
using System.Web.Hosting;
using SportEvents.Models;
using System.Collections.Generic;

namespace SportEvents
{
    public class HangFireCronTest
    {
        private DataContext db = new DataContext();
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

        public void SentMessageToAllUnspokenUsers()
        {
            List<User> Users = new List<User>();
            string body = "Zasíláme upozornění, že jste se ještě nevyjádřil k  události";
            string subject = "Upozornění na událost";




            Users = db.GetAllUnspokenUsers(1);
            bool response = false;
            foreach (var item in Users)
            {
                EmailService service = new EmailService();
                response = service.Send(item.Email, subject, body);

            }



        }
    }
}