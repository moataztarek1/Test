using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KidZone.Domain.Entities
{
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public ICollection<ContentCategory> ContentCategories { get; set; } = new HashSet<ContentCategory>();
    }
}
