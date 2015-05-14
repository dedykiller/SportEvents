using SportEvents.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SportEvents.ViewModels;
using System.Data.Entity;

namespace SportEvents.Controllers
{
    public class PaymentPeriodController : Controller
    {
        private DataContext db = new DataContext();

        // GET: Edit
        public ActionResult Edit(int? groupId)
        {
            if (groupId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // nelze převést int do int?, tak si vypomáhám tímhle
            int x = groupId.GetValueOrDefault();

            PaymentPeriod @PaymentPeriod = db.GetActualPaymentPeriod(x);
            @PaymentPeriod.GroupId = x;
            if (@PaymentPeriod == null)
            {
                return HttpNotFound();
            }
            
            return View(@PaymentPeriod);
        }

        // POST: PaymentPeriod/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "End,Start,Id,GroupId")] PaymentPeriod @PaymentPeriod)
        {
            if (ModelState.IsValid)
            {

                
                db.Entry(@PaymentPeriod).State = EntityState.Modified;
                
                PaymentPeriod NextPaymentPeriod = new PaymentPeriod();
                NextPaymentPeriod = db.GetNextPaymentPeriod(@PaymentPeriod);
                if (NextPaymentPeriod != null)
                {
                    NextPaymentPeriod.Start = @PaymentPeriod.End.AddDays(1);
                    if (NextPaymentPeriod.End < NextPaymentPeriod.Start)
                    {
                        NextPaymentPeriod.End = NextPaymentPeriod.Start.AddDays(30);
                    } 
                }

                if (@PaymentPeriod.End <= @PaymentPeriod.Start)
                {
                    TempData["notice"] = "Další zúčtovací období musí končit později než začíná.";
                    return View(@PaymentPeriod);
                }
                db.SaveChanges();
               // return RedirectToAction("Index");
                return RedirectToAction("Details", "Groups", new { id = PaymentPeriod.GroupId });
            }
            
            return View(@PaymentPeriod);
        }

        // GET: PaymentPeriod
        public ActionResult CreateNextPaymentPeriod(int? groupId)
        {
            int x = groupId.GetValueOrDefault();
            if (groupId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (db.IsAlreadyDefinedNextPaymentPeriodInThisGroup(x) == true)
            {
                

                TempData["notice"] = "Následující účtovacé období již máte definováno, můžete ho vytvořit až po uplynutí aktuálního účtovacího období";
                return RedirectToAction("Details", "Groups", new { id = x });

            }
            
            PaymentPeriod PaymentPeriod = new PaymentPeriod();
            // nelze převést int do int?, tak si vypomáhám tímhle
            
            // začátek nového období je den po konci aktuálního
            PaymentPeriod.Start = db.GetActualPaymentPeriod(x).End.AddDays(1);
            PaymentPeriod.GroupId = x;
            PaymentPeriod.GroupName = db.GetGroupById(x).Name;              
            return View(PaymentPeriod);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNextPaymentPeriod([Bind(Include = "Start,End,GroupId")] PaymentPeriod PaymentPeriod)
        {
            User user = (User)Session["UserSession"];
            int a = PaymentPeriod.GroupId;
            
            if (PaymentPeriod.End > PaymentPeriod.Start)
            {
                // je další zúčtovací období definováno?
                if (db.IsAlreadyDefinedNextPaymentPeriodInThisGroup(PaymentPeriod.GroupId )== false)
                {
                    // jestli není, tak ho vytvoří a vypíše hlášku
                    db.PaymentPeriods.Add(PaymentPeriod);
                    db.SaveChanges();

                    foreach (var UserInGroup in db.AllUsersInGroup(PaymentPeriod.GroupId))
                    {
                        db.SetDefaultTypeOfPaymentForUser(UserInGroup, PaymentPeriod);
                    }

                    TempData["notice"] = "Následující účtovacé období od " +PaymentPeriod.Start + " do: "+ PaymentPeriod.End.Date +  " bylo vytvořeno uživatelem " + user.Email;
                    return RedirectToAction("Details", "Groups", new { id = PaymentPeriod.GroupId });
                
                }
                else
                {
                    // jestli je, tak jen vypiš hlášku
                
                    TempData["notice"] = "Následující účtovacé období již máte definováno, můžete ho vytvořit až po uplynutí aktuálního účtovacího období";
                    return RedirectToAction("Details", "Groups", new { id = PaymentPeriod.GroupId });
                }

            }
            else
            {
                TempData["notice"] = "Další zúčtovací období musí končit později než začíná.";
                return View(PaymentPeriod);

            }          
            
        }

        public ActionResult Index (int groupId) 
        {
            PaymentPeriodCollectionVM vm = new PaymentPeriodCollectionVM();
            vm.PaymentPeriods = db.GetAllPaymentPeriodsOfGroup(groupId);


            return View(vm);
        }
       
        public ActionResult ListOfPayments(int groupId, int PaymentPeriodId) 
        {
            
            ListOfPaymentsForUserInPaymentPeriodVM vm = new ListOfPaymentsForUserInPaymentPeriodVM();
            vm.SumPrices = 0;
            vm.SumCash = 0;
            vm.SumAfterPeriod = 0;
            vm.Events = db.GetAllEventsOfPaymentPeriod(PaymentPeriodId);
            List<User> AllUsersInGroup = db.AllUsersInGroup(groupId);
            List<User> AllUsersPayingCash = db.GetAllUsersPayingByCashOrAfterPeriod(AllUsersInGroup, PaymentPeriodId, TypeOfPaymentInPeriod.Cash);
            List<User> AllUsersPayingAfterPeriod = db.GetAllUsersPayingByCashOrAfterPeriod(AllUsersInGroup, PaymentPeriodId, TypeOfPaymentInPeriod.AfterPeriod);
            vm.ChargedUsersPayingByCash = db.GetAllChargedUsers(AllUsersPayingCash, vm.Events, PaymentPeriodId);
            vm.ChargedUsersPayingAfterPeriod = db.GetAllChargedUsers(AllUsersPayingAfterPeriod, vm.Events, PaymentPeriodId);
            
            foreach (var item in vm.ChargedUsersPayingByCash)
            {
                if (item != null)
                {
                    item.EventsParticipationYes = db.GetEventsWhereIsThisParticipation(vm.Events, participation.Yes, item.Id);
                    item.sum = item.EventsParticipationYes.Select(x => x.Price).Sum();
                    vm.SumCash += item.sum;
                }
               
            }

            foreach (var item in vm.ChargedUsersPayingAfterPeriod)
            {
                if (item != null)
                {
                    item.EventsParticipationYes = db.GetEventsWhereIsThisParticipation(vm.Events, participation.Yes, item.Id);
                    item.sum = item.EventsParticipationYes.Select(x => x.Price).Sum();
                    vm.SumAfterPeriod += item.sum;
                }

            }
            
            vm.SumPrices = vm.SumCash + vm.SumAfterPeriod;

            vm.PaymentPeriod = db.PaymentPeriods.Where(x => x.Id == PaymentPeriodId).Single();
            vm.PaymentPeriod.GroupName = db.Groups.Where(x => x.Id == vm.PaymentPeriod.GroupId).Single().Name;
            
            return View(vm);
        }


    }
}