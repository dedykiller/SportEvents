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
using SportEvents.ViewModels;

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

            ArticleCommentsVM vm = new ArticleCommentsVM();
            vm.Article = article;
            vm.Comments = db.getAllCommentsOfArticle(id);
            //vm.Comments = db.Comments.ToList(); všechny komentáře, ne pouze pro daný článek

            return View(vm);
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
        public ActionResult Create([Bind(Include = "ID,Title,Body,GroupID")] Article article, HttpPostedFileBase uploadFile)
        {
            if (ModelState.IsValid)
            {
                string filePathName = null;

                if (uploadFile.ContentLength > 204800) // 200 kb limit pro obrázek
                {
                    ModelState.AddModelError("uploadFile", "Maximální velikost souboru je 200 Kb");
                    return View(article);
                }

                var supportedTypes = new[] { "jpg", "jpeg", "png", "JPG", "JPEG", "PNG" };

                var fileExt = System.IO.Path.GetExtension(uploadFile.FileName).Substring(1);

                if (!supportedTypes.Contains(fileExt))
                {
                    ModelState.AddModelError("uploadFile", "Špatný formát obrázku. Pouze formáty jpg, jpeg a png jsou podporovány.");
                    return View(article);
                }

                if (uploadFile != null && uploadFile.ContentLength > 0)
                {
                    string fileContentType = uploadFile.ContentType;
                    filePathName = Path.GetFileName(uploadFile.FileName);
                    byte[] fileBytes = new byte[uploadFile.ContentLength];
                    uploadFile.InputStream.Read(fileBytes, 0, Convert.ToInt32(uploadFile.ContentLength));
                    var path = Path.Combine(Server.MapPath(ImagesPath), filePathName);
                    uploadFile.SaveAs(path);
                    TempData["upload"] = "Soubor " + filePathName + " typu " + fileContentType + " byl načten a uložen";
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

                //Group g = db.Groups.Find(article.GroupID);
                //string subject = string.Format("Upozornění na nový článek");
                //string body = string.Format("Byl přidán nový článek s názvem : <b>{0}</b> od uživatele : <b>{1}</b> ve skupině : <b>{2}</b> <br/><br/>Váš ERASMUS team", article.Title, article.CreatorFullName, g.Name);

                //List<User> users = db.AllUsersInGroup(g.Id);
                //bool kq = false;

                //foreach (User item in users)
                //{
                //    string emailTo = item.Email;
                //    EmailService service = new EmailService();
                //    kq = service.Send(emailTo, subject, body);
                //}


                //TempData["email"] = "Uživatel " + user.Email + " byl přidán do systému a byl odeslán potvrzovací e-mail: " + kq;
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
        public ActionResult Edit([Bind(Include="ID,Title,Body,GroupID")] Article article, HttpPostedFileBase uploadFile)
        {
            if (ModelState.IsValid)
            {
                string filePathName = null;

                if (uploadFile.ContentLength > 204800) // 200 kb limit pro obrázek
                {
                    ModelState.AddModelError("uploadFile", "Maximální velikost souboru je 200 Kb");
                    return View(article);
                }

                var supportedTypes = new[] { "jpg", "jpeg", "png", "JPG", "JPEG", "PNG" };

                var fileExt = System.IO.Path.GetExtension(uploadFile.FileName).Substring(1);

                if (!supportedTypes.Contains(fileExt))
                {
                    ModelState.AddModelError("uploadFile", "Špatný formát obrázku. Pouze formáty jpg, jpeg a png jsou podporovány.");
                    return View(article);
                }

                if (uploadFile != null && uploadFile.ContentLength > 0)
                {
                    string fileContentType = uploadFile.ContentType;
                    filePathName = Path.GetFileName(uploadFile.FileName);
                    byte[] fileBytes = new byte[uploadFile.ContentLength];
                    uploadFile.InputStream.Read(fileBytes, 0, Convert.ToInt32(uploadFile.ContentLength));
                    var path = Path.Combine(Server.MapPath(ImagesPath), filePathName);
                    uploadFile.SaveAs(path);
                    TempData["upload"] = "Soubor " + filePathName + " typu " + fileContentType + " byl načten a uložen";
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
