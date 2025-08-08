using KidZone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KidZone.Domain.Entities
{
    public class Content
    {
        [Key]
        public int ContentID { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public ContentAccessType ContentType { get; set; }

        public DateTime? CreatedAt { get; set; }

        public virtual Video? Video { get; set; }
        public virtual Story? Story { get; set; }

        public ICollection<ContentCategory> ContentCategories { get; set; } = new HashSet<ContentCategory>();
        public ICollection<Rating> Ratings { get; set; } = new HashSet<Rating>();
        public ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
        public ICollection<Favorite> Favorites { get; set; } = new HashSet<Favorite>();
    }

}
