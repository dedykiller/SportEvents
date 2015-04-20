using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SportEvents.Models
{
    
    public class Group
    {
        public Group()
        {
            EventList = new List<Event>();
        }
        public int Id { get; set; }
        public int Creator { get; set; }

        [Display(Name = "fillGroupName", ResourceType = typeof(SportEvents.Languages.Resources))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "pleaseFillName", ErrorMessageResourceType = typeof(SportEvents.Languages.Resources))]
        public string Name { get; set; }

        [Display(Name = "enterDescripGroup", ResourceType = typeof(SportEvents.Languages.Resources))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "pleaseFillDescrip", ErrorMessageResourceType = typeof(SportEvents.Languages.Resources))]
        public string Description { get; set; }
        public DateTime CreateTime { get; set; }

        //[NotMapped]
        //[DataType(DataType.Date)]
        //public DateTime StartOfPaymentPeriod { get; set; }
        
        [NotMapped]
        [DataType(DataType.Date)]
        public DateTime EndOfPaymentPeriod { get; set; }

        [Display(Name = "enterTypePay", ResourceType = typeof(SportEvents.Languages.Resources))]
      //  public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<Event> Events { get; set; }
        public int NumberOfUsersInGroup { get; set; }
        public virtual ICollection<PaymentPeriod> PaymentPeriods { get; set; }
        
        //public Group()
        //{
        //    Users = new HashSet<User>();
        //    Events = new HashSet<Event>();
        //}






        public List<Event> EventList { get; set; }
    }
}