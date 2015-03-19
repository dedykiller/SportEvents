﻿using System;
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

        [DisplayName("Zadej název skupiny")]
        [Required(ErrorMessage = "Vyplňte prosím název skupiny")]
        public string Name { get; set; }

        [DisplayName("Zadej popis skupiny")]
        [Required(ErrorMessage = "Vyplňte prosím popis skupiny")]
        public string Description { get; set; }
        public DateTime CreateTime { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartOfPaymentPeriod { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime EndOfPaymentPeriod { get; set; }
                
        [DisplayName("Zadej typ platby pro následující účtovací období")]
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