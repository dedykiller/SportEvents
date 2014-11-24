﻿using System;
using System.Collections.Generic;
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
    }
}