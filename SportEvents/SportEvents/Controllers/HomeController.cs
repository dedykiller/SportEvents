
using SportEvents.Controllers.Utility;
using SportEvents.Models;
using SportEvents.Models.Application;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace SportEvents.Controllers
{
    public class HomeController : Controller
    {
        private UsersBO usersBO = new UsersBO();

        // GET: /Home/Index
        public ActionResult Index()
        {
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
                    return RedirectToAction("index", "Groups");
                }
                else
                {
                    ViewBag.Error = "Neexistující uživatel nebo chybné heslo";
                    return View();
                }
            }
            return View();
        }

        //GET: Home/Logout

        public ActionResult Logout()
        {
            Session["UserSession"] = null; // vynulování session

            return RedirectToAction("Index", "Home");
        }
	}
}