using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KidZone.Domain.Entities
{
    public class Story
    {
        [Key]
        public int StoryID { get; set; }
        public string Text_Content { get; set; }
        public string? ImgUrl { get; set; }
        public int ContentID { get; set; }
        [ForeignKey("ContentID")]
        public Content? Content { get; set; }
    }

}
