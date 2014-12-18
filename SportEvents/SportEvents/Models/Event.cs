using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [Required(ErrorMessage = "Vyplňte prosím název události")]
        public string Name { get; set; }
        public DateTime TimeOfEvent { get; set; }
        public string Place { get; set; }
        public string Description { get; set; }

        [RegularExpression("^[0-9][0-9]*$",
            ErrorMessage = "Cena události musí být kladné číslo.")]

        public int Price { get; set; }
        public  TypeOfPayment Payment { get; set;}
        public virtual ICollection<Group> Groups { get; set; }

        public Event()
        {
            Groups = new HashSet<Group>();
        }
    }

    
}