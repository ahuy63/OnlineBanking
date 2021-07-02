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

        public int? CurrencyId { get; set; }
        public Currency Currency { get; set; }

        public string Description { get; set; }
        public double Amount { get; set; }
        public double NewBalanceSender { get; set; }
        public double NewBalanceRecipient { get; set; }

        public DateTime IssuedDate { get; set; }
        public bool Status { get; set; }


        //Thêm list vào cho dễ truy vấn
        public List<Notification> Notifications { get; set; }
    }
}
