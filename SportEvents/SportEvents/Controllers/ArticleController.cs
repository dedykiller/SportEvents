using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SportEvents.Models;
using System.IO;

namespace SportEvents.Views
{
    public class ArticleController : Controller
    {
        private DataContext db = new DataContext();
        private const string ImagesPath = "~/Image";

        // GET: /Article/
        public ActionResult Index()
        {
            return View(db.Articles.ToList());
        }

        // GET: /Article/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Article article = db.Articles.Find(id);
            if (article == null)
            {
                return HttpNotFound();
            }
            return View(article);
        }

        // GET: /Article/Create
        public ActionResult Create(int id)
        {
            Article article = new Article();
            article.GroupID = id;

            return View(article);
        }

        // POST: /Article/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Title,Body,GroupID")] Article article)
        {
            if (ModelState.IsValid)
            {
                string filePathName = null;

                if (Request != null)
                {
                    HttpPostedFileBase file = Request.Files["UploadedFile"];

                    if ((file != null) && (file.ContentLength > 0) && ((file.ContentType == "image/png") || (file.ContentType == "image/jpg")) &&
                        (file.ContentLength < 100000) && !string.IsNullOrEmpty(file.FileName))
                    {
                        string fileContentType = file.ContentType;
                        filePathName = Path.GetFileName(file.FileName);
                        byte[] fileBytes = new byte[file.ContentLength];
                        file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                        var path = Path.Combine(Server.MapPath(ImagesPath), filePathName);
                        file.SaveAs(path);
                        TempData["upload"] = "Soubor " + filePathName + " typu " + fileContentType + " byl načten a uložen";
                    }
                }

                User user = (User)Session["UserSession"];
                article.UserID = user.Id;
                article.CreatorFullName = user.FirstName + " " + user.Surname;
                article.CreationTime = DateTime.Now;

                if (filePathName != null)
                {
                    article.Picture = ImagesPath + "/" + filePathName;
                }
                else
                {
                    article.Picture = ImagesPath + "/no_image.png";
                }
                db.Articles.Add(article);
                db.SaveChanges();

                Group g = db.Groups.Find(article.GroupID);
                string subject = string.Format("Upozornění na nový článek");
                string body = string.Format("Byl přidán nový článek s názvem : <b>{0}</b> od uživatele : <b>{1}</b> ve skupině : <b>{2}</b>, přečíst si ho můžete <a href=\"http://localhost:3922/?redirect=http://localhost:3922/Article/\">zde</a> <br/><br/>Váš ERASMUS team", article.Title, article.CreatorFullName, g.Name, article.ID);

                List<User> users = db.AllUsersInGroup(g.Id);
                bool kq = false;

                foreach (User item in users)
                {
                    string emailTo = item.Email;
                    EmailService service = new EmailService();
                    kq = service.Send(emailTo, subject, body);
                }


                TempData["email"] = "Uživatel " + user.Email + " byl přidán do systému a byl odeslán potvrzovací e-mail: " + kq;
                TempData["notice"] = "Uživatel " + article.CreatorFullName + " vložil článek : " + article.Title;

                return RedirectToAction("Details", "Groups", new { id = article.GroupID });
            }


            return View(article);
        }


        // GET: /Article/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Article article = db.Articles.Find(id);
            if (article == null)
            {
                return HttpNotFound();
            }
            return View(article);
        }

        // POST: /Article/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,Title,Body,GroupID")] Article article)
        {
            if (ModelState.IsValid)
            {
                string filePathName = null;

                if (Request != null)
                {
                    HttpPostedFileBase file = Request.Files["UploadedFile"];

                    if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                    {
                        string fileContentType = file.ContentType;
                        filePathName = Path.GetFileName(file.FileName);
                        byte[] fileBytes = new byte[file.ContentLength];
                        file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                        var path = Path.Combine(Server.MapPath(ImagesPath), filePathName);
                        file.SaveAs(path);
                        TempData["upload"] = "Soubor " + filePathName + " typu " + fileContentType + " byl načten a uložen";
                    }
                }

                if (filePathName != null)
                {
                    article.Picture = ImagesPath + "/" + filePathName;
                }
                else
                {
                    article.Picture = ImagesPath + "/no_image.png";
                }

                article.CreationTime = DateTime.Now;
                User u = (User)Session["UserSession"];
                article.CreatorFullName = u.FirstName + " " + u.Surname;
                article.UserID = u.Id;
                db.Entry(article).State = EntityState.Modified;
                db.SaveChanges();

                TempData["notice"] = "Uživatel " + article.CreatorFullName + " úspěšně editoval článek : " + article.Title;
                return RedirectToAction("Details", "Groups", new { id = article.GroupID });
            }
            return View(article);
        }

        // GET: /Article/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Article article = db.Articles.Find(id);
            if (article == null)
            {
                return HttpNotFound();
            }
            return View(article);
        }

        // POST: /Article/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Article article = db.Articles.Find(id);
            db.Articles.Remove(article);
            db.SaveChanges();
            return RedirectToAction("Details", "Groups", new { id = article.GroupID });
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
