using SportEvents.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace SportEvents.Controllers
{
    public class PaymentPeriodController : Controller
    {
        private DataContext db = new DataContext();
        // GET: PaymentPeriod
        public ActionResult CreateNextPaymentPeriod(int? groupId)
        {
            if (groupId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //int x = groupId.GetValueOrDefault();
            //PaymentPeriod @PaymentPeriod = db.GetActualPaymentPeriod(x);
            PaymentPeriod PaymentPeriod = new PaymentPeriod();
            // nelze převést int do int?, tak si vypomáhám tímhle
            int x = groupId.GetValueOrDefault();
            PaymentPeriod.Start = db.GetActualPaymentPeriod(x).End.AddDays(1);
            PaymentPeriod.GroupId = x;
           // @PaymentPeriod.End = DateTime.Now;
            PaymentPeriod.GroupName = db.GetGroupById(x).Name;
            //ViewBag.ActualPeriod.Start = db.GetActualPaymentPeriod(x).Start;
            //ViewBag.ActualPeriod.End = db.GetActualPaymentPeriod(x).End;
            return View(PaymentPeriod);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNextPaymentPeriod([Bind(Include = "Start,End,GroupId")] PaymentPeriod PaymentPeriod)
        {
            User user = (User)Session["UserSession"];
            int a = PaymentPeriod.GroupId;
            if (db.IsAlreadyDefinedNextPaymentPeriodInThisGroup(PaymentPeriod.GroupId )== false)
            {
                db.PaymentPeriods.Add(PaymentPeriod);
                db.SaveChanges();
                TempData["notice"] = "Následující účtovacé období od " +PaymentPeriod.Start + " do: "+ PaymentPeriod.End.Date +  " bylo vytvořeno uživatelem " + user.Email;
                return RedirectToAction("Details", "Groups", new { id = PaymentPeriod.GroupId });
                
            }
            else
            {
                
                TempData["notice"] = "Následující účtovacé období již máte definováno, můžete ho vytvořit až po uplynutí aktuálního účtovacího období";
                return RedirectToAction("Details", "Groups", new { id = PaymentPeriod.GroupId });
            }
            
            
        }
    }
}