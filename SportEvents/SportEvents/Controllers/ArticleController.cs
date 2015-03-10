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
        private const string ImagesPath = "~/Resources/images";

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
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Article/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Title,Body")] Article article)
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

                        //Response.ContentType = "application/jpeg";
                        //Response.AddHeader("Content-Disposition", @"filename=""jpg_test.jpg""");
                        //Response.TransmitFile(@"~\Resources\images\jpg_test.jpg");

                        TempData["upload"] = "Soubor " + filePathName + " typu " + fileContentType + " byl načten";
                    }
                }

                User user = (User)Session["UserSession"];
                article.UserID = user.Id;
                if (filePathName != null)
                {
                    article.Picture = ImagesPath + "/" + filePathName;
                }
                else
                {
                    article.Picture = ImagesPath + "/jpg_test.jpg";
                }
                db.Articles.Add(article);
                db.SaveChanges();

                TempData["notice"] = "Uživatel " + user.FirstName + " vložil článek " + article.Title;

                return RedirectToAction("Index");
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
        public ActionResult Edit([Bind(Include="ID,Title,Body")] Article article)
        {
            if (ModelState.IsValid)
            {
                db.Entry(article).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
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
