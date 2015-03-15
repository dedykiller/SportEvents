using Foolproof;
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
        

        [DataType(DataType.Currency)]
        [RegularExpression(@"^\d+.\d{0}$",ErrorMessage = "Cena musí být kladné číslo")]
        public decimal Price { get; set; }
        public bool Repeat { get; set; } // opakovana udalost? ano x ne

        [Range(1, 2)]      
        public int Interval { get; set; } // interval opakovani udalosti v tydnech
        public virtual Group Group { get; set; }
       
        public virtual ICollection<UsersInEvent> UserInEvents { get; set; }

        [NotMapped]
        public participation participation { get; set; }
        

        
    }

    
}