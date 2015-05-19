using SportEvents.Controllers;
using SportEvents.Models.Application;
using SportEvents.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace SportEvents.Models
{
    public class GroupsController : BaseController
    {

        GroupsBO groupsBO = new GroupsBO();

        // GET: /Groups/
        public ActionResult Index()
        {
            return View(groupsBO.Index());
        }

        public ActionResult IndexCreator()
        {
            User user = (User)Session["UserSession"];
            return View(groupsBO.IndexCreator(user.Id));
        }

        public ActionResult IndexMember()
        {
            User user = (User)Session["UserSession"];
            return View(groupsBO.IndexMember(user.Id));
        }

        // GET: /Groups/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            User user = (User)Session["UserSession"];
            Group group = groupsBO.GetGroupById(id);

            GroupEventsArticlesVM vm = new GroupEventsArticlesVM();
            vm.Group = group;

            if (groupsBO.IsUserInGroup(user.Id, group.Id))
            {
                vm.Events = groupsBO.AllEventsOfGroup(group.Id);
                ViewBag.IsUserInGroup = (bool)true;
                vm.Articles = groupsBO.GetAllArticlesOfGroup(group.Id);
            }
            else
            {
                List<Event> Events = new List<Event>();
                vm.Events = Events;
                List<Article> Articles = new List<Article>();
                vm.Articles = Articles;
                ViewBag.IsUserInGroup = (bool)false;
            }
            if (group.Creator == user.Id)
            {
                ViewBag.IsUserCreator = (bool)true;
            }
            else
            {
                ViewBag.IsUserCreator = (bool)false;

            }


            return View(vm);
        }

        //public ActionResult ChangeTypeOfPaymentForPeriod ()
        //{
        //    return View();
        //}
        //[HttpPost]
        //public ActionResult ChangeTypeOfPaymentForPeriod(string ahoj)
        //{
        //    return View();
        //}

        // GET: /Groups/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Groups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include = "Id,Name,Description,EndOfPaymentPeriod")] Group group)
        {
            if (ModelState.IsValid)
            {
                if (Session["UserSession"] != null)
                {
                    User user = (User)Session["UserSession"];

                    groupsBO.CreateGroup(group, user);
                    TempData["notice"] = "Skupina " + group.Name + " byla úspěšně vytvořena uživatelem " + user.Email;

                    return RedirectToAction("Index");

                }
            }

            return View(group);
        }

        // GET: /Groups/AddUserToGroup/5
        public ActionResult AddUserToGroup(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = groupsBO.GetGroupById(id);

            return View(group);
        }



        // POST: /Groups/AddUserToGroup/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Details")]
        public ActionResult Details(int id)
        {
            if (ModelState.IsValid)
            {
                if (Session["UserSession"] != null)
                {

                    User user = (User)Session["UserSession"];
                    Group group = groupsBO.GetGroupById(id);

                    if (groupsBO.IsUserInGroup(user.Id, group.Id))
                    {
                        TempData["notice"] = "Uživatel " + user.FirstName + " už je členem skupiny " + group.Name;
                    }
                    else
                    {
                        groupsBO.AddUserToGroup(group, user);
                        TempData["notice"] = "Uživatel " + user.FirstName + " byl přidán do skupiny " + group.Name;
                    }

                    return RedirectToAction("Details", "Groups", new { id = group.Id });
                }
            }

            TempData["notice"] = "Uživatel není přihlášený";
            return View();
        }


        // GET: /Groups/RemoveUserFromGroup/5
        public ActionResult RemoveUserFromGroup(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = groupsBO.GetGroupById(id);

            return View(group);
        }

        // POST: /Groups/RemoveUserFromGroup/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("RemoveUserFromGroup")]
        public ActionResult RemoveUserFromGroup(int id)
        {
            if (ModelState.IsValid)
            {
                if (Session["UserSession"] != null)
                {

                    User user = (User)Session["UserSession"];
                    Group group = groupsBO.GetGroupById(id);

                    if (groupsBO.IsUserInGroup(user.Id, group.Id))
                    {
                        groupsBO.RemoveUserFromGroup(group, user);
                        TempData["notice"] = "Uživatel " + user.FirstName + " se úspěšně odhlásil ze skupiny : " + group.Name;
                        return RedirectToAction("Details", "Groups", new { id = group.Id });
                    }
                    else
                    {
                        TempData["notice"] = "Uživatel " + user.FirstName + " již není ve skupině : " + group.Name;
                        return RedirectToAction("Details", "Groups", new { id = group.Id });
                    }

                    
                }
            }

            TempData["notice"] = "Uživatel není přihlášený";
            return View();
        }

        // GET: /Groups/CloseGroup/5
        public ActionResult CloseGroup(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = groupsBO.GetGroupById(id);

            return View(group);
        }

        // POST: /Groups/CloseGroup/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("CloseGroup")]
        public ActionResult CloseGroup(int id)
        {
            if (ModelState.IsValid)
            {
                if (Session["UserSession"] != null)
                {

                    User user = (User)Session["UserSession"];
                    Group group = groupsBO.GetGroupById(id);

                    if (group.Creator == user.Id && group.IsOpened==true)
                    {
                        groupsBO.CloseGroup(group);
                        TempData["notice"] = "Uživatel " + user.FirstName + " úspěšně uzavřel skupinu : " + group.Name;
                        return RedirectToAction("Details", "Groups", new { id = group.Id });
                    }
                    else
                    {
                        TempData["notice"] = "Skupina " + group.Name + " již je uzavřená";
                        return RedirectToAction("Details", "Groups", new { id = group.Id });
                    }


                }
            }

            TempData["notice"] = "Uživatel není přihlášený";
            return View();
        }

        // GET: /Groups/OpenGroup/5
        public ActionResult OpenGroup(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = groupsBO.GetGroupById(id);

            return View(group);
        }

        // POST: /Groups/OpenGroup/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("OpenGroup")]
        public ActionResult OpenGroup(int id)
        {
            if (ModelState.IsValid)
            {
                if (Session["UserSession"] != null)
                {

                    User user = (User)Session["UserSession"];
                    Group group = groupsBO.GetGroupById(id);

                    if (group.Creator == user.Id && group.IsOpened == false)
                    {
                        groupsBO.OpenGroup(group);
                        TempData["notice"] = "Uživatel " + user.FirstName + " úspěšně otevřel skupinu : " + group.Name;
                        return RedirectToAction("Details", "Groups", new { id = group.Id });
                    }
                    else
                    {
                        TempData["notice"] = "Skupina " + group.Name + " již je otevřená";
                        return RedirectToAction("Details", "Groups", new { id = group.Id });
                    }


                }
            }

            TempData["notice"] = "Uživatel není přihlášený";
            return View();
        }

        // GET: /Groups/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = groupsBO.GetGroupById(id);

            return View(group);
        }

        // POST: /Groups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Creator,CreatorFullName, Name, Description, NumberOfUsersInGroup")] Group group)
        {
            if (ModelState.IsValid)
            {
                User user = (User)Session["UserSession"];
                group.Creator = user.Id;
                groupsBO.EditGroup(group);

                TempData["notice"] = "Správce skupiny úspěšně editoval skupinu : " + group.Name;
                return RedirectToAction("Details", "Groups", new { id = group.Id });
            }
            return View(group);
        }

        // GET: /Groups/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = groupsBO.GetGroupById(id);

            return View(group);
        }

        // POST: /Groups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Group group = groupsBO.GetGroupById(id);

            groupsBO.DeleteGroup(group);

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                groupsBO.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
