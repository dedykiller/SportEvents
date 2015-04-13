using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SportEvents.Models
{
    public class Article
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int GroupID { get; set; }
        public string Title { get; set; }
        [DataType(DataType.MultilineText)]
        public string Body { get; set; }
        public string Picture { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreatorFullName{ get; set; }
        [NotMapped]
        public virtual ICollection<Comment> Comments{ get; set; }

        public Article()
        {
            Comments = new HashSet<Comment>();
        }
    }
}