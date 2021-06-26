using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineBanking.Models
{
    //[Index(nameof(Number), IsUnique = true)]
    public class Account
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public int? AccountTypeId { get; set; }
        public AccountType AccountType { get; set; }

        [DisplayName("Account Number")]
        [Required(ErrorMessage = ("{0} Account Number Cannot Be Blank"))]
        public string Number { get; set; }
        public int Balance { get; set; }
        public DateTime CreateDate { get; set; }

        public bool Status { get; set; }


        //Thêm list vào truy vấn cho dễ
        public List<Cheque> Cheques { get; set; }

        
        public List<Transaction> FromTransactions { get; set; }

        public List<Transaction> ToTransactions { get; set; }

        public List<AddressBook> AddressBooks { get; set; }
    }
}
