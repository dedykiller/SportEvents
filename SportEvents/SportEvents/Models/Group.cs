using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SportEvents.Models
{
    public enum TypeOfPayment
    {
       
        Cash,
       AfterPeriod
    }
    public class Group
    {
        public int Id { get; set; }
        public virtual User Creator { get; set; }

        [DisplayName("Zadej název skupiny")]
        [Required(ErrorMessage = "Vyplňte prosím název skupiny")]
        public string Name { get; set; }

        [DisplayName("Zadej popis skupiny")]
        [Required(ErrorMessage = "Vyplňte prosím popis skupiny")]
        public string Description { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime StartOfPaymentPeriod { get; set; }
        public DateTime EndOfPaymentPeriod { get; set; }

        [DisplayName("Zadej délku následujícího účtovacího období v měsících")]
        [Required(ErrorMessage = "Vyplňte prosím délku účtovacího období")]
        [RegularExpression("^[1-9][0-9]*$",
            ErrorMessage = "Počet měsíců musí být větší než nula.")]
        public int PaymentPeriodLength { get; set; }

        [DisplayName("Zadej typ platby pro následující účtovací období")]
        public TypeOfPayment Payment { get; set;  }
        public virtual ICollection<User> Users { get; set; }

        public Group()
        {
            Users = new HashSet<User>();
        }



    }
}