using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineBanking.Models
{
    public class Currency
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double ExchangeRate { get; set; }
        public bool Status { get; set; }
    }
}
