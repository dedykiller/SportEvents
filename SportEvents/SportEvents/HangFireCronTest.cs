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
            List<UsersInEvent> UsersInEvents = new List<UsersInEvent>();
            string body = "";
            string subject = "Upozornění na událost";
            int[] days = { 5, 3, 2, 1 };


            for (int i = 0; i < days.Length; i++)
            {
                UsersInEvents = db.GetAllUnspokenUsers(days[i]);
                bool response = false;
                // TODO p5epsat

                foreach (var item in UsersInEvents)
                {
                    User User = db.GetUserById(item.UserId);
                    Event Event = db.GetEventById(item.EventId);
                    EmailService service = new EmailService();
                    body = "Zasíláme upozornění, že jste se ještě nevyjádřil k  události " + Event.Name + " konané " +"dne " + Event.TimeOfEvent.ToShortDateString() + " v " + Event.TimeOfEvent.ToShortTimeString();
                    response = service.Send(User.Email, subject, body);

                }
            }




        }
    }
}