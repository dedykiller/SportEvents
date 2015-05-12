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
            vm.Comments = db.getAllCommentsByParent(article.ID, ParentType.Article); // vrátí všechny komenty pro dané ID článku
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
                
                if (uploadFile != null)
                {
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

                    filePathName = Path.GetFileName(uploadFile.FileName);
                    byte[] fileBytes = new byte[uploadFile.ContentLength];
                    uploadFile.InputStream.Read(fileBytes, 0, Convert.ToInt32(uploadFile.ContentLength));
                    var path = Path.Combine(Server.MapPath(ImagesPath), filePathName);
                    uploadFile.SaveAs(path);
                    article.Picture = ImagesPath + "/" + filePathName;
                    
                }
                else
                {
                    article.Picture = ImagesPath + "/no_image.png";
                }

                User user = (User)Session["UserSession"];
                article.UserID = user.Id;
                article.CreatorFullName = user.FirstName + " " + user.Surname;
                article.CreationTime = DateTime.Now;

                db.Articles.Add(article);
                db.SaveChanges();

                Group g = db.Groups.Find(article.GroupID);
                string subject = string.Format("Upozornění na nový článek");
                string body = string.Format("Byl přidán nový článek s názvem : <b>{0}</b> od uživatele : <b>{1}</b> ve skupině : <b>{2}</b>, přečíst si ho můžete <a href=\"http://sportevents.aspone.cz?redirect=http://sportevents.aspone.cz/Article/Details/{3}\">zde</a> <br/><br/>Váš ERASMUS team", article.Title, article.CreatorFullName, g.Name, article.ID);
                //string body = string.Format("Byl přidán nový článek s názvem : <b>{0}</b> od uživatele : <b>{1}</b> ve skupině : <b>{2}</b>, přečíst si ho můžete <a href=\"http://localhost:3922/?redirect=http://localhost:3922/Article/Details/{3}\">zde</a> <br/><br/>Váš ERASMUS team", article.Title, article.CreatorFullName, g.Name, article.ID);

                List<User> users = db.AllUsersInGroup(g.Id);
                bool response = false;

                foreach (User item in users)
                {
                    EmailService service = new EmailService();
                    response = service.Send(item.Email, subject, body);
                }


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
        public ActionResult Edit([Bind(Include="ID,Title,Body,GroupID,Picture")] Article article, HttpPostedFileBase uploadFile)
        {
            if (ModelState.IsValid)
            {
                string filePathName = null;

                if (uploadFile != null)
                {
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

                    filePathName = Path.GetFileName(uploadFile.FileName);
                    byte[] fileBytes = new byte[uploadFile.ContentLength];
                    uploadFile.InputStream.Read(fileBytes, 0, Convert.ToInt32(uploadFile.ContentLength));
                    var path = Path.Combine(Server.MapPath(ImagesPath), filePathName);
                    uploadFile.SaveAs(path);
                    article.Picture = ImagesPath + "/" + filePathName;

                }

                User user = (User)Session["UserSession"];
                article.UserID = user.Id;
                article.CreatorFullName = user.FirstName + " " + user.Surname;
                article.CreationTime = DateTime.Now;
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
            TempData["notice"] = "Článek " + article.Title + " byl úspěšně smazán";
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
