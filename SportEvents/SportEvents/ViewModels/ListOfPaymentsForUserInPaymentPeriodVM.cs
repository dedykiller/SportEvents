using SportEvents.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SportEvents.ViewModels
{
    public class ListOfPaymentsForUserInPaymentPeriodVM
    {
        public PaymentPeriod PaymentPeriod { get; set; }
        public List<User> ChargedUsersPayingByCash { get; set; }
        public List<User> ChargedUsersPayingAfterPeriod { get; set; }
        
        public List<Event> Events { get; set; }
        public int SumPrices { get; set; }
        public int SumCash { get; set; }
        public int SumAfterPeriod { get; set; }
    }
}