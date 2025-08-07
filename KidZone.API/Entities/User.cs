using KidZone.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KidZone.Domain.Entities
{
    public class User:IdentityUser<int>
    {
        public string FullName { get; set; }
        public string? Gender { get; set; }

        public ICollection<Child> Children { get; set; } = new HashSet<Child>();
        public ICollection<UserSubscription> Subscriptions { get; set; } = new HashSet<UserSubscription>();
        public ICollection<Favorite> Favorites { get; set; } = new HashSet<Favorite>();
        public ICollection<Rating> Ratings { get; set; } = new HashSet<Rating>();
        public ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    }

}
