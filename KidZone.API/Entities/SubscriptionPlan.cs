using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KidZone.Domain.Entities
{
    public class SubscriptionPlan
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int DurationInMonths { get; set; }

        public ICollection<UserSubscription> UserSubscriptions { get; set; }
    }

}
