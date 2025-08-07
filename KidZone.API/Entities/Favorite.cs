using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KidZone.Domain.Entities
{
    public class Favorite
    {
        public int ID { get; set; }

        public int? UserID { get; set; }
        [ForeignKey("UserID")]
        public User? User { get; set; }
        public int? ChildID { get; set; }
        [ForeignKey("ChildID")]
        public Child? Child { get; set; }
        public int ContentID { get; set; }
        [ForeignKey("ContentID")]
        public Content? Content { get; set; }
    }

}
