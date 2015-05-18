using Hangfire;
using SportEvents.Languages;
using SportEvents.Models;
using SportEvents.Models.Application;
using System;
using System.Web;
using System.Web.Mvc;

namespace SportEvents.Controllers
{
    public class HomeController : BaseController
    {
        private UsersBO usersBO = new UsersBO();

        // GET: /Home/Index
        public ActionResult Index()
        {
            HangFireCronTest hgf = new HangFireCronTest();
            
            // Test zapisu na disk, acc denied - proc?
            // RecurringJob.AddOrUpdate(() => hgf.TestHangFire(), Cron.Minutely);

            // Test, jestli se to odesle normalne - failed
            //hgf.TestEmailCron();
            //hgf.SentMessageToAllUnspokenUsers();

            // Test jobu, prida se do success, ale email neprijde
            RecurringJob.AddOrUpdate(() => hgf.SentMessageToAllUnspokenUsers(), Cron.Daily);
           // RecurringJob.AddOrUpdate(() => hgf.TestEmailCron(), Cron.Minutely);
            RecurringJob.AddOrUpdate(() => hgf.CreateNextPaymentPeriodIfNot(), Cron.Minutely);

            

            return View();
        }

        //POST: /Home/Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(Login login) // Post metoda pro prihlaseni do systemu
        {
            if (ModelState.IsValid)
            {
                if (usersBO.IsUserRegistered(login.Email, login.Password))
                {
                    User user = usersBO.GetUser(login);
                    Session["UserSession"] = user;
                    TempData["notice"] = "Uživatel " + login.Email + " byl úspěšně přihlášen";
                    if (!String.IsNullOrEmpty(Request.QueryString["redirect"]))
                    {
                        string url = Request.QueryString["redirect"];
                        Response.Redirect(url);
                    }
                    return RedirectToAction("index", "Groups");
                }
                else
                {
                    TempData["notice"] = "Neexistující uživatel nebo chybné heslo.";
                    ViewBag.Error = "Neexistující uživatel nebo chybné heslo";
                    return View();
                }
            }
            return View();
        }

        //GET: Home/Logout

        public ActionResult Logout()
        {
            User user = (User)Session["UserSession"];
            TempData["notice"] = "Uživatel " + user.FirstName + " " + user.Surname +  " byl úspěšně odhlášen";
            Session["UserSession"] = null; // vynulování session

            return RedirectToAction("Index", "Home");
        }

        public ActionResult SetCulture(string culture)
        {
            // Validate input
            culture = CultureHelper.GetImplementedCulture(culture);

            // Save culture in a cookie
            HttpCookie cookie = Request.Cookies["_culture"];
            if (cookie != null)
                cookie.Value = culture;   // update cookie value
            else
            {
                cookie = new HttpCookie("_culture");
                cookie.Value = culture;
                cookie.Expires = DateTime.Now.AddYears(1);
            }
            Response.Cookies.Add(cookie);

            return Redirect(Request.UrlReferrer.ToString());
        }
    }
}