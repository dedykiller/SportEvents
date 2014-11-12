using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SportEvents.Models
{
    public class DataContext : DbContext
    {
        public DataContext()
            : base("SportEventsDB")
        {
        }

        public DbSet<User> Users { get; set; }
        
        }




}