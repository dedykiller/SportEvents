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
        public int Id { get; set; }
        public virtual User Creator { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<User> Users { get; set; }

        public Group()
        {
            Users = new HashSet<User>();
        }



    }
}