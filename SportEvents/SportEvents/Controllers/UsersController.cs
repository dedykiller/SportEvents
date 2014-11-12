using SportEvents.Controllers.Utility;
using SportEvents.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace SportEvents.Controllers
{
    public class UsersController : Controller
    {
        private DataContext db = new DataContext();
        private bool emailFound = false;

        // GET: Users/Login

        public ActionResult Login()
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
           
            //TO-DO správně se vypisující errory, metoda na porovnání hashovaných hesel zvlášť, oddělit dotazování od controlleru a předávat usera
            
        }

        //GET: Users/Logout
        public ActionResult Logout()
        {
            Session["LoginSession"] = null; // vynulování session

            return RedirectToAction("Index");
        }

        // GET: Users
        
        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            return View();
        }



        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User user)
        {
            if (ModelState.IsValid)
            {
                
                user.RegistrationTime = DateTime.Now; // Vytvoření data registrace
                user.Password = UtilityMethods.CalculateHashMd5(user.Password); // Zahashování hesla
                user.PasswordComparison = UtilityMethods.CalculateHashMd5(user.PasswordComparison);

                emailFound = db.Users.Any(x => x.Email == user.Email); //Vyhodnotí, zda je zadaný e-mail v databázi
                if (emailFound) // Pokud databáze zadaný e-mail obsahuje, vrátí nás na formulář pro registraci
                {
                    ViewBag.Error = "Uživatel pod tímto emailem je již registrován";
                    return View();
                }
                else
                {
                    db.Users.Add(user); // uložení uživatele a uložení změn v tabulce
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }

            }

            return View(user);
   
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Email,Password,FirstName,Surname,Telephone,RegistrationTime")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}