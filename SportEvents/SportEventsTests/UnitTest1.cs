using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SportEvents.Controllers.Utility;
using System.Text.RegularExpressions;
using SportEvents.Models;

namespace SportEventsTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void HashTest()
        {
            String hashedPassword = "25D55AD283AA400AF464C76D713C07AD";
            String hashedActual = UtilityMethods.CalculateHashMd5("12345678");

            Assert.AreEqual(hashedPassword, hashedActual);
        }

        [TestMethod]
        public void EmailRegularExpTest()
        {
            String regExpEmail = "^[_A-Za-z0-9-\\+]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9-]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$";

            Assert.IsTrue(Regex.IsMatch("namesmt@example.com", regExpEmail));
            Assert.IsTrue(Regex.IsMatch("name.smt@example.com", regExpEmail));
            Assert.IsTrue(Regex.IsMatch("namesxmfdddddddddddddddasdft@exampldfsssssssssafdsfsde.com", regExpEmail));
            Assert.IsTrue(Regex.IsMatch("n@student.osu.cz", regExpEmail));
        }

        [TestMethod]
        public void EmailRegularExpFailTest()
        {
            String regExpEmail = "^[_A-Za-z0-9-\\+]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9-]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$";

            Assert.IsFalse(Regex.IsMatch("namesmt@examplecom", regExpEmail));
            Assert.IsFalse(Regex.IsMatch("name.smtexample.com", regExpEmail));
            Assert.IsFalse(Regex.IsMatch("namesmt@example.c", regExpEmail));
            Assert.IsFalse(Regex.IsMatch("n.f.d.f@@a.ma", regExpEmail));
        }

        [TestMethod]
        public void RegisterEmailNotification()
        {

            string emailTo = "KuznikJan@seznam.cz";
            string subject = string.Format("Potvrzení registrace");
            string body = string.Format("Děkujeme Vám za Vaši registraci <b>{0}</b>:)<br/><br/>Váš ERASMUS team", "Jane");

            EmailService service = new EmailService();

            Assert.IsTrue(service.Send(emailTo, subject, body));
        }
    }
}
