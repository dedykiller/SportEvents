using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SportEvents.Models
{
    public enum ParentType
    {
        Article = 0,
        Event = 1,
    }

    public class Comment
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int ParentID { get; set; }
        public ParentType ParentType { get; set; }
        [MaxLength(100, ErrorMessage = "Délka textu je maximálně 100 znaků")]
        public string Text { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreatorFullName{ get; set; }
    }
}