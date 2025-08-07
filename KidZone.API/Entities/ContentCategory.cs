using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KidZone.Domain.Entities
{
    public class ContentCategory
    {
        public int ContentID { get; set; }
        [ForeignKey("ContentID")]
        public Content Content { get; set; }
        public int CategoryID { get; set; }
        [ForeignKey("CategoryID")]
        public Category? Category { get; set; }
    }

}
