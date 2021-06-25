using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineBanking.Models
{
    [Index(nameof(Name),IsUnique =true)]
    public class AccountType
    {
        public int Id { get; set; }

        [DisplayName("Name Of Account Type")]
        [Required(ErrorMessage = "{0} Name Of Account Type Cannot Be Blank")]
        public string Name { get; set; }
        public bool Status { get; set; }

        //Thêm các list để dễ truy vấn
        public List<Account> Accounts { get; set; }
    }
}
