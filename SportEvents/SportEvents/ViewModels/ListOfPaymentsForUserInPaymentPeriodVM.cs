using SportEvents.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SportEvents.ViewModels
{
    public class ListOfPaymentsForUserInPaymentPeriodVM
    
    {
        [DisplayFormat(DataFormatString = "{0:#} Kč")]
        public PaymentPeriod PaymentPeriod { get; set; }
        public List<User> ChargedUsersPayingByCash { get; set; }
        public List<User> ChargedUsersPayingAfterPeriod { get; set; }
        
        public List<Event> Events { get; set; }
        public decimal SumPrices { get; set; }
        public decimal SumCash { get; set; }
        public decimal SumAfterPeriod { get; set; }
    }
}