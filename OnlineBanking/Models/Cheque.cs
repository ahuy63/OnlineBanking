using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineBanking.Models
{
    public class Cheque
    {
        public int Id { get; set; }
        public int? AccountId { get; set; }
        public Account Accounts { get; set; }

        public DateTime DateCreate { get; set; }

        [Required(ErrorMessage = "{0} Address Cannot Be Blank")]
        public string Address { get; set; }
        public double Amount { get; set; }
        public int CurrencyId { get; set; }
        public Currency currency { get; set; }
        public bool Status { get; set; }
    }
}
