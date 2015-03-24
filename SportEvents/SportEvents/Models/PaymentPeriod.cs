using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SportEvents.Models
{
    public class PaymentPeriod
    {
        public int Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public Group Group { get; set; }

    }
}