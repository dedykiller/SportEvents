using SportEvents.Controllers.Utility;
using SportEvents.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace SportEvents.Controllers
{
    public class LoginController : Controller
    {
        private DataContext db = new DataContext();
        private bool emailFound = false;

        // GET: /Login/
        public ActionResult Index()
        {
            return View();
        }

        //POST: Users/Login

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Login login)
        {
            if (ModelState.IsValid)
            {
                emailFound = db.Users.Any(x => x.Email == login.Email); //Vyhodnotí, zda je zadaný e-mail v databázi

                if (!emailFound) // Pokud databáze e-mail neobsahuje, vrátí nás na formulář pro přihlášení
                {
                    ViewBag.Error = "Neexistující uživatel nebo chybné heslo";
                    return View();
                }

                string hashedFormPassword = UtilityMethods.CalculateHashMd5(login.Password); // Zahashování hesla z loginu
                string hashedDBPassword = db.Users.Where(x => x.Email == login.Email) // Dotaz pro získání zahashovaného hesla z databáze
                                                .Select(x => x.Password)
                                                .Single();

                if (!hashedDBPassword.Equals(hashedFormPassword)) // Pokud se hashované hesla neshodují, uživatel se přepošle na přihlášení
                {
                    ViewBag.Error = "Neexistující uživatel nebo chybné heslo";
                    return View();
                }

            }

            Session["LoginSession"] = login.Email; // Vytvoření Session prozatím jen s loginem uživatele
            return RedirectToAction("Index");

            //TODO : správně se vypisující errory, metoda na porovnání hashovaných hesel zvlášť, oddělit dotazování od controlleru a předávat usera

        }
	}
}