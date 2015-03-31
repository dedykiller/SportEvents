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
    //nic
    public class EventsController : Controller
    {
        private DataContext db = new DataContext();

        // GET: Events
        public ActionResult Index(string sortOrder)
        {
            
            User user = (User)Session["UserSession"];
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date" : "Date";
            var events = from s in db.Events select s;
            switch (sortOrder)
            {
                case "name":
                    events = events.OrderBy(s => s.Name);
                    break;
                case "date":
                    events = events.OrderBy(s => s.TimeOfEvent);
                    break;
                default :
                    
                    break;
    
            }
            //return View(db.AllEventsWhereIsUserCreator(user.Id)); pokud chceme vratit jen udalosti, kde je clovek zakladatel
            return View(@events.ToList());
        }

        // GET: Events/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = (User)Session["UserSession"];
            Event @event = db.Events.Find(id);
            @event.participation = db.GetParticipation(@event.Id, user.Id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            
            
            @event.UserParticipationYes = db.UsersInEventParticipation(@event.Id, participation.Yes);
            @event.UserParticipationNo = db.UsersInEventParticipation(@event.Id, participation.No);
            @event.UserParticipationUnspoken = db.UsersInEventParticipation(@event.Id, participation.Unspoken);
            return View(@event);
        }

        [HttpPost]
        public ActionResult Details ([Bind(Include ="participation,Id")]Event @event)
        {
            User user = (User)Session["UserSession"];
            db.UpdateParticipation(@event.Id, user.Id, @event.participation);
            @event = db.GetEventById(@event.Id);
            @event.UserParticipationYes = db.UsersInEventParticipation(@event.Id, participation.Yes);
            @event.UserParticipationNo = db.UsersInEventParticipation(@event.Id, participation.No);
            @event.UserParticipationUnspoken = db.UsersInEventParticipation(@event.Id, participation.Unspoken);

            
           
            
            return View(@event);
        }

        // GET: Events/Create
        public ActionResult Create()
        {
            
            User user = (User)Session["UserSession"];
            if (db.HasUserAnyGroupWhereIsCreator(user.Id))
            {
                ViewBag.GrpId = new SelectList(db.AllGroupsWhereIsUserCreator(user.Id), "Id","Name");
                return View();
            }
            //ViewBag.GrpId = new SelectList(db.Groups, "Id", "Name");
            else
            {
                TempData["notice"] = "Uživatel " + user.Email + " není zakladatelem žádné skupiny. Nejdříve musíte vytvořit skupinu." ;
                return RedirectToAction("Index", "Groups");
            }
        }


        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,TimeOfEvent,RepeatUntil,GrpId,Place,Description,Price,Repeat,Interval,CreatorId")] Event @event)
        {
            

            if (ModelState.IsValid)
            {
                User user = (User)Session["UserSession"];
                @event.CreatorId = user.Id;
              
                List<int> eventsIds = new List<int>();
              
                
                if (db.IsUserCreatorOfGroup(user.Id, @event.GrpId))
                {
                    
                    if (@event.Repeat) 
                    {
                      
                        for (DateTime dT = @event.TimeOfEvent ; dT <= @event.RepeatUntil; dT = dT.AddDays(7*@event.Interval)) {
                            @event.UserInEvents = null;
                            @event.TimeOfEvent = dT;
                            Event extraEvent = new Event();
                            extraEvent = @event;

                            db.Events.Add(extraEvent);
                            db.SaveChanges();

                            eventsIds.Add(@event.Id);
  
                            //@event.TimeOfEvent = @event.TimeOfEvent.AddDays(7*@event.Interval); 


                        }                       
                    }
                    else
                    {
                        db.Events.Add(@event);                        
                        db.SaveChanges();                        
                        eventsIds.Add(@event.Id);                       
                    }
                    db.AddAllUsersOfGroupToAllNewEvents(db.GetEventByIdToList(eventsIds), @event.GrpId);
                    TempData["notice"] = "Událost " + @event.Name + " byla vytvořena uživatelem " + user.Email;

                    return RedirectToAction("Details", "Groups", new {id = @event.GrpId});
                    
                }                    
                else
                {
                    TempData["notice"] = "Uživatel " + user.Email + " není zakladatelem skupiny s ID:" + @event.GrpId;
                    return RedirectToAction("Index");
                    
                }
                
                
            }
            

            ViewBag.GrpId = new SelectList(db.Groups, "Id", "Name", @event.GrpId);
            ViewBag.Error = "Nejste zakladatelem tehle skupiny";
            return View(@event);
        }

        public void AddUsersToAllNewEvents(List<int> listOfEventsId)
        {
            
            List<Event> e = new List<Event>();
            e = db.SelectEventsById(listOfEventsId);
            var x = e.First();
            List<User> users = db.AllUsersInGroup(x.GrpId);
            foreach (var us in users)
            {
                foreach (var ev in e)
                {
                    db.AddUserToEvent(us, ev);
                }
                

            }
        }

        // GET: Events/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            ViewBag.GrpId = new SelectList(db.Groups, "Id", "Name", @event.GrpId);
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,TimeOfEvent,RepeatUntil,GrpId,Place,Description,Price,Repeat,Interval")] Event @event)
        {
            if (ModelState.IsValid)
            {
                
                db.Entry(@event).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.GrpId = new SelectList(db.Groups, "Id", "Name", @event.GrpId);
            return View(@event);
        }

        // GET: Events/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Event @event = db.Events.Find(id);
            db.Events.Remove(@event);
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
