using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KidZone.Domain.Entities
{
    public class Video
    {
        public int VideoID { get; set; }
        public string Url { get; set; }
        public TimeSpan? Duration { get; set; }

        public int ContentID { get; set; }
        [ForeignKey("ContentID")]
        public Content? Content { get; set; }
    }

}
