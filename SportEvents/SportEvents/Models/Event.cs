using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SportEvents.Models
{
    public enum TypeOfPayment 
    {
        Cash,
        AfterPeriod

    }
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime TimeOfEvent { get; set; }
        public string Place { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public  TypeOfPayment Payment { get; set;}
        public virtual ICollection<Group> Groups { get; set; }

        public Event()
        {
            Groups = new HashSet<Group>();
        }
    }

    
}