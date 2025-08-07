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
    public class Payment
    {
        public int ID { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod Method { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTime Date { get; set; }

        public int User_Sub_ID { get; set; }
        public UserSubscription? Subscription { get; set; }
    }

}
