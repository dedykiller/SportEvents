using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SportEvents.Models
{
    
    // jen pro komit
    public class Event
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Vyplňte prosím název události")]
        [MaxLength(25, ErrorMessage = "Délka názvu musí být maximálně 25 znaků")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Vyplňte prosím čas a datum události")]
        public DateTime TimeOfEvent { get; set; }
        
        public DateTime? RepeatUntil { get; set; }
        public int GrpId { get; set; }
        public int CreatorId { get; set; }
       
        public string Place { get; set; }
        public string Description { get; set; }
        [RegularExpression("^[0-9][0-9]*$",
            ErrorMessage = "Cena události musí být kladné číslo.")]

        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
        public bool Repeat { get; set; } // opakovana udalost? ano x ne

        [Range(1, 2)]      
        public int Interval { get; set; } // interval opakovani udalosti v tydnech
        public virtual Group Group { get; set; }
       
        public virtual ICollection<UsersInEvent> UserInEvents { get; set; }

        

        
    }

    
}