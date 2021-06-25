using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineBanking.Models
{   
    public class Transaction
    {
        public int Id { get; set; }

        [ForeignKey("FromAccount")]
        public int? FromAccountId { get; set; }

        [InverseProperty("FromTransactions")]
        public Account FromAccount { get; set; }


        [ForeignKey("ToAccount")]
        public int? ToAccountId { get; set; }

        [InverseProperty("ToTransactions")]
        public Account ToAccount { get; set; }


        public int Amount { get; set; }
        public int NewBalance { get; set; }
        
        public DateTime IssuedDate { get; set; }
        public bool Status { get; set; }

        //Thêm list vào cho dễ truy vấn
        public List<Notification> Notifications { get; set; }
    }
}
