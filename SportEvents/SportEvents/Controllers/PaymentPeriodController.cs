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
            
            PaymentPeriod PaymentPeriod = new PaymentPeriod();
            // nelze převést int do int?, tak si vypomáhám tímhle
            int x = groupId.GetValueOrDefault();
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


            // je další zúčtovací období definováno?
            if (PaymentPeriod.End > PaymentPeriod.Start)
            {
                
            
                if (db.IsAlreadyDefinedNextPaymentPeriodInThisGroup(PaymentPeriod.GroupId )== false)
                {
                    // jestli není, tak ho vytvoří a vypíše hlášku
                    db.PaymentPeriods.Add(PaymentPeriod);
                    db.SaveChanges();
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
    }
}