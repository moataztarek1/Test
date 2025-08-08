using KidZone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KidZone.Domain.Entities
{
    public class Rating
    {
        [Key]
        public int RatingID { get; set; }
        public RatingType Type { get; set; } // like , dislike
        public string UserID { get; set; }
        [ForeignKey("UserID")]
        public User? User { get; set; }

        public int ContentID { get; set; }
        [ForeignKey("ContentID")]
        public Content? Content { get; set; }
    }

}
