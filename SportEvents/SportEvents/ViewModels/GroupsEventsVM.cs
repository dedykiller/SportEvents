using SportEvents.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SportEvents.ViewModels
{
    public class GroupsEventsVM
    {
        public Group Group { get; set; }
        public List<Event> Events { get; set; }
    }
}