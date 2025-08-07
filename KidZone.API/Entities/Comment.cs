using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KidZone.Domain.Entities
{
    public class Comment
    {
        [Key]
        public int CommentID { get; set; }
        public string TextComment { get; set; }
        public DateTime CommentDate { get; set; }

        public int UserID { get; set; }
        [ForeignKey("UserID")]
        public User? User { get; set; }

        public int ContentID { get; set; }
        [ForeignKey("ContentID")]
        public Content Content { get; set; }
    }

}
