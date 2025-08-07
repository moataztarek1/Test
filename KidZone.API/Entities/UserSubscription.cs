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
    public class UserSubscription
    {
        [Key]
        public int ID { get; set; }
        public int UserID { get; set; } 
        public int Sub_Plan_ID { get; set; }
        public SubscriptionStatus Status { get; set; }
        public DateTime Start_Date { get; set; }
        public DateTime End_Date { get; set; }

        public SubscriptionPlan? Plan { get; set; }
        public ICollection<Payment> Payments { get; set; }
    }

}
