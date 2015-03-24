using Owin;
using SportEvents.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace SportEvents.Controllers.Utility
{
    public class UtilityMethods
    {
        private DataContext db = new DataContext();
        public static string CalculateHashMd5(string input)
        {
            MD5 md5 = MD5.Create();
            byte[] inputToBeHashed = Encoding.ASCII.GetBytes(input);

            byte[] hashed = md5.ComputeHash(inputToBeHashed);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashed.Length; i++)
            {
                sb.Append(hashed[i].ToString("X2"));
            }

            return sb.ToString();

        }

        public static bool ComparePasswords(string loginPassword, string dbPassword)
        {
            if(loginPassword.Equals(dbPassword))
            {
                return true;
            }

            return false;
        }

        public void CreateNewPaymentPeriodByCron()
        {
            List<PaymentPeriod> periods = new List<PaymentPeriod>();
            periods = db.PaymentPeriods.Where(x => x.End == (DateTime.Today)).ToList();

            foreach (var item in periods)
            {
                db.PaymentPeriods.Add(new PaymentPeriod
                {
                    Start = item.End.AddDays(1),
                    End = item.End.AddYears(10),
                    Group = item.Group

                });
                db.SaveChanges();
            }
        }
    }
}