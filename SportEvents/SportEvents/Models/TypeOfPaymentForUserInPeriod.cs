using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SportEvents.Models
{
    public enum TypeOfPaymentInPeriod  
    { 
        Cash = 0,
        AfterPeriod = 1,
    }
    public class TypeOfPaymentForUserInPeriod
    {
        [Key, Column(Order = 0)]
        public int UserId { get; set; }
        [Key, Column(Order = 1)]
        public int PaymentPeriodId { get; set; }
        public TypeOfPaymentInPeriod UserTypeOfPaymentInPeriod { get; set; }
        public virtual User User { get; set; }
        public virtual PaymentPeriod PaymentPeriod { get; set; }

    }
}