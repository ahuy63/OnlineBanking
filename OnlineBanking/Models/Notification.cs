using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineBanking.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public User User { get; set; }
        public int? TransactionId { get; set; }
        public Transaction Transaction { get; set; }
        public string Message { get; set; }
        public DateTime CreateDate { get; set; }
        public bool HaveRead { get; set; }
        
    }
}
