using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineBanking.Models
{
    public class AddressBook
    {
        public int Id { get; set; }

        public int? UserId { get; set; }
        public User User { get; set; }

        public int? AccountId { get; set; }
        public Account Account { get; set; }

    }
}
