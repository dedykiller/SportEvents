using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SportEvents.Models
{
    public class PaymentPeriod
    {
        public int Id { get; set; }
        [DataType(DataType.Date)]
        [DisplayName("Začátek období")]
        public DateTime Start { get; set; }
        [DisplayName("Konec období")]
        [DataType(DataType.Date)]
        public DateTime End { get; set; }
        public int GroupId { get; set; }
        [NotMapped]
        public string GroupName { get; set; }
        public virtual ICollection<TypeOfPaymentForUserInPeriod> TypeOfPaymentsForUsersInPeriods { get; set; }

    }
}