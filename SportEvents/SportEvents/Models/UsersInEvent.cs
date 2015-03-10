using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SportEvents.Models
{
    public enum participation
    {
        Unspoken = 0,
        No = -1,
        Yes = 1,
        
        
    }
    public class UsersInEvent
    {
     //   [Key, Column(Order = 0)]
        public int UserId { get; set; }
     //   [Key, Column(Order = 1)]
        public int EventId { get; set; }        
        public participation participation { get; set; }
        public virtual User User { get; set; }
        public virtual Event Event { get; set; }

    }
}