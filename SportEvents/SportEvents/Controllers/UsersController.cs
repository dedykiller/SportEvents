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
    public class UsersController : BaseController
    {
        private DataContext db = new DataContext();
        private UsersBO usersBO = new UsersBO();

        // GET: Users/ListOfUsers
        public ActionResult ListOfUsers()
        {
            return View(db.Users.ToList());
        }

        // GET: Users/Registration
        public ActionResult Register()
        {
            return View();
        }


        // POST: Users/Registration
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                if (usersBO.IsEmailInDatabase(user.Email)) // Pokud databáze zadaný e-mail obsahuje, vrátí nás na formulář pro registraci
                {
                    ViewBag.Error = "Uživatel pod tímto emailem je již registrován";
                    return View(user);
                }

                usersBO.RegisterUser(user);

                string subject = string.Format("Potvrzeni registrace");
                string body = string.Format("Děkujeme Vám za Vaši registraci <b>{0}</b>:)<br/><br/>Váš ERASMUS team", user.FirstName);

                EmailService service = new EmailService();
                bool response = service.Send(user.Email, subject, body);

                if (response == true)
                {
                    TempData["notice"] = "Uživatel " + user.FirstName + " byl přidán do systému a byl zaslán potvrzovací e-mail na adresu : " + user.Email;
                }
                else
                {
                    TempData["notice"] = "Uživatel " + user.FirstName + " byl přidán do systému, ale potvrzovací e-mail se nepodařilo poslat";
                }

                return RedirectToAction("index", "Home");
                
            }

            return View();

        }

        public ActionResult EditAccount()
        {
            //load information about login user to edit form
            User user = (User)Session["UserSession"];
            User LogginUser = db.Users.Single(p => p.Id == user.Id);

            if (LogginUser == null)
                return HttpNotFound();

            return View(LogginUser);
        }

        // POST: /Product/EditAccount
        [HttpPost]

        public ActionResult EditAccount(User user)
        {

            User changedUser = db.Users.Single(p => p.Id == user.Id);

            //record changed values
            changedUser.Email = user.Email;
            changedUser.RegistrationTime = DateTime.Now;
            changedUser.Password = user.Password;
            changedUser.PasswordComparison = user.Password;
            changedUser.FirstName = user.FirstName;
            changedUser.Surname = user.Surname;
            changedUser.Telephone = user.Telephone;


            user = changedUser;
            db.SaveChanges();
            TempData["chUserValues"] = "Vaše údaje byly změněny";
            return RedirectToAction("EditAccount");
        }

        // GET: Users/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }


        // POST: Users/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(User user)
        {
            User userP = (User)Session["UserSession"];
            User changedPassword = db.Users.Single(p => p.Id == userP.Id);
            string originPassword = changedPassword.Password;

            changedPassword.Password = user.Password;
            changedPassword.PasswordComparison = user.Password;

            user = changedPassword;

            if (user.Password != originPassword)
            {
                user.Password = UtilityMethods.CalculateHashMd5(user.Password);
                user.PasswordComparison = UtilityMethods.CalculateHashMd5(user.PasswordComparison);
            }
            db.SaveChanges();

            TempData["chPassword"] = "Vaše heslo bylo změněno";
            return RedirectToAction("ChangePassword");

        }

        // GET: Users/ForgotPassword
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //POST: Users/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult ForgotPassword(ForgotPassword user)
        {
            if (ModelState.IsValid)
            {
                if (usersBO.IsEmailInDatabase(user.Email)) // Pokud databáze zadaný e-mail obsahuje, vrátí nás na formulář pro registraci
                {
                    String genPassword = GeneratePasswd.Generate(8, 10);
                    String newPassword = UtilityMethods.CalculateHashMd5(genPassword);

                    User memberUser = db.GetUserByEmail(user.Email);

                    memberUser.Email = memberUser.Email;
                    memberUser.RegistrationTime = DateTime.Now;
                    memberUser.Password = newPassword;
                    memberUser.PasswordComparison = newPassword;
                    memberUser.FirstName = memberUser.FirstName;
                    memberUser.Surname = memberUser.Surname;
                    memberUser.Telephone = memberUser.Telephone;

                    db.SaveChanges();

                    //odeslani emailu

                    string emailTo = user.Email;
                    string subject = string.Format("Potvrzení registrace");
                    string body = string.Format("Vaše nové heslo je " + genPassword);

                    EmailService service = new EmailService();

                    bool kq = service.Send(emailTo, subject, body);

                    TempData["notice"] = "Uživateli " + user.Email + " bylo odesláno nové heslo";
                    //return RedirectToAction("index", "Home");
                    return RedirectToAction("index", "Home");
                }

                ViewBag.Error = "Uživatel pod tímto emailem je již registrován";
                return View();

            }

            return View();
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
                return RedirectToAction("ListOfUsers");
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
            return RedirectToAction("ListOfUsers");
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