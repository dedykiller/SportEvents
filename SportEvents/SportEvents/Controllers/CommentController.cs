using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SportEvents.Models;

namespace SportEvents.Controllers
{
    public class CommentController : Controller
    {
        private DataContext db = new DataContext();

        // GET: /Comment/
        public ActionResult Index()
        {
            return View(db.Comments.ToList());
        }

        // GET: /Comment/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // GET: /Comment/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Comment/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ParentID,ParentType,Text")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                comment.CreationTime = DateTime.Now;
                User user = (User)Session["UserSession"];
                comment.UserID = user.Id;
                comment.CreatorFullName = user.FirstName + " " + user.Surname;
                db.Comments.Add(comment);
                db.SaveChanges();

                TempData["notice"] = "Uživatel " + user.FirstName + " úspěšně přidal komentář";
                switch(comment.ParentType){
                    case ParentType.Article:
                        return RedirectToAction("Details", "Article", new { id = comment.ParentID });
                    case ParentType.Event:
                        return RedirectToAction("Details", "Events", new { id = comment.ParentID });    
                    default:
                        break;
                }  
            }

            TempData["notice"] = "Komentář se nepodařilo vytvořit, pravděpodobně je moc dlouhý";
            if(comment.ParentType == ParentType.Article){
                return RedirectToAction("Details", "Article", new { id = comment.ParentID });
            }else{
                return RedirectToAction("Details", "Events", new { id = comment.ParentID }); 
            }
        }

 

        // GET: /Comment/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }


        // POST: /Comment/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit([Bind(Include = "ID,ParentID,ParentType,Text")]  Comment comment)
        {
            if (ModelState.IsValid)
            {
                comment.CreationTime = DateTime.Now;
                User user = (User)Session["UserSession"];
                comment.UserID = user.Id;
                comment.CreatorFullName = user.FirstName + " " + user.Surname;
                db.Comments.Add(comment);
                db.Entry(comment).State = EntityState.Modified;
                db.SaveChanges();

                TempData["notice"] = "Úspěšná editace komentáře";
                switch (comment.ParentType)
                {
                    case ParentType.Article:
                        return RedirectToAction("Details", "Article", new { id = comment.ParentID });
                    case ParentType.Event:
                        return RedirectToAction("Details", "Events", new { id = comment.ParentID });
                    default:
                        break;
                }  
            }

            TempData["notice"] = "Komentář se nepodařilo vytvořit, pravděpodobně je moc dlouhý";
            if (comment.ParentType == ParentType.Article)
            {
                return RedirectToAction("Details", "Article", new { id = comment.ParentID });
            }
            else
            {
                return RedirectToAction("Details", "Events", new { id = comment.ParentID });
            }
        }

        // GET: /Comment/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }


        // POST: /Comment/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Comment comment = db.Comments.Find(id);
            db.Comments.Remove(comment);
            db.SaveChanges();

            TempData["notice"] = "Komentář " + comment.Text + " byl úspěšně smazán";
            switch (comment.ParentType)
            {
                case ParentType.Article:
                    return RedirectToAction("Details", "Article", new { id = comment.ParentID });
                case ParentType.Event:
                    return RedirectToAction("Details", "Events", new { id = comment.ParentID });
                default:
                    break;
            }

            return View();
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
