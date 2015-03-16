﻿using SportEvents.Controllers.Utility;
using SportEvents.Models;
using SportEvents.Models.Application;
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
                    return View();
                }

                usersBO.RegisterUser(user);

                string smtpUserName = "sportevents1@seznam.cz";
                string smtpPassword = "777003862";
                string smtpHost = "smtp.seznam.cz";
                int smtpPort = 25;

                string emailTo = user.Email;
                string subject = string.Format("Potvrzeni registrace");
                string body = string.Format("Děkujeme Vám za Vaši registraci <b>{0}</b>:)<br/><br/>Váš ERASMUS team", user.FirstName);

                EmailService service = new EmailService();

                bool kq = service.Send(smtpUserName, smtpPassword, smtpHost, smtpPort, emailTo, subject, body);

                TempData["notice"] = "Uživatel " + user.Email + " byl přidán do systému a byl odeslán potvrzovací e-mail: " + kq;
                return RedirectToAction("ListOfUsers");
                
            }

            return View();

        }

        // TODO : 

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