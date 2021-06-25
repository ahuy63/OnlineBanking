using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnlineBanking.Models;
using Microsoft.EntityFrameworkCore;

namespace OnlineBanking.Data
{
    public class OnlineBankingContext : DbContext
    {
        public OnlineBankingContext(DbContextOptions<OnlineBankingContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountType> AccountTypes { get; set; }
        public DbSet<Cheque> Cheques { get; set; }
        public DbSet<AddressBook> AddressBooks { get; set; }
        public DbSet<Transaction> Transaction { get; set; }
        public DbSet<Notification> Notifications { get; set; }

    }
}
