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
using SportEvents.Controllers.Utility;
using Hangfire;
using System.Diagnostics;

namespace SportEvents.Models
{
    
    public class GroupsController : Controller
    {        
        GroupsBO groupsBO = new GroupsBO();

        // GET: /Groups/
        public ActionResult Index()
        {
            //RecurringJob.AddOrUpdate(() => Debug.WriteLine("pico pico pico" + DateTime.Now), Cron.Minutely);
            RecurringJob.AddOrUpdate(()=> UtilityMethods.CreateNewPaymentPeriodByCron)

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

            GroupsEventsVM vm = new GroupsEventsVM();
            vm.Group = group;

            if(groupsBO.IsUserInGroup(user.Id, group.Id))
            {
                vm.Events = groupsBO.AllEventsOfGroup(group.Id);
                ViewBag.IsUserInGroup = (bool) true;
            }
            else
            {
                List<Event> Events = new List<Event>();
                vm.Events = Events;
                ViewBag.IsUserInGroup = (bool)false;
            }
           
            return View(vm);
        }

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
                    }else
                    {
                        groupsBO.AddUserToGroup(group, user);
                        TempData["notice"] = "Uživatel " + user.FirstName + " byl přidán do skupiny " + group.Name;
                    }

                    return RedirectToAction("Index");  
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
        public ActionResult Edit([Bind(Include="Id,Name,Description")] Group group)
        {
            if (ModelState.IsValid)
            {
                groupsBO.EditGroup(group);                
                return RedirectToAction("Index");
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
