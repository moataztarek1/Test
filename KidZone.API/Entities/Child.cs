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
    public class Child
    {
        [Key]
        public int ChildID { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string FName { get; set; }
        public string? LName { get; set; }
        public AgeGroup Age_Group { get; set; }
        public string? AvatarUrl { get; set; }

        public int ParentID { get; set; }
        [ForeignKey("ParentID")]
        public User? Parent { get; set; }

        public ICollection<Favorite> Favorites { get; set; } = new HashSet<Favorite>();
    }

}
