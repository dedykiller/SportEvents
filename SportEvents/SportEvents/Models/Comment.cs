using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SportEvents.Models
{
    public class Comment
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int ArticleID { get; set; }
        public string Text { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreatorFullName{ get; set; }

    }
}