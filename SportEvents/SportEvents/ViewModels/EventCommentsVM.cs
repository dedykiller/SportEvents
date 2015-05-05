using SportEvents.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SportEvents.ViewModels
{
    public class EventCommentsVM
    {
        public Event Event { get; set; }
        public List<Comment> Comments { get; set; }

    }
}