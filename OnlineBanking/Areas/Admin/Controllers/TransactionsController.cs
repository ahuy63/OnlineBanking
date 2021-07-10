using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineBanking.Data;
using OnlineBanking.Models;

namespace OnlineBanking.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TransactionsController : Controller
    {
        private readonly OnlineBankingContext _context;

        public TransactionsController(OnlineBankingContext context)
        {
            _context = context;
        }

        // GET: Admin/Transactions
        public async Task<IActionResult> Index()
        {
            var onlineBankingContext = _context.Transactions.Include(t => t.Currency).Include(t => t.FromAccount).Include(t => t.ToAccount);

            return View(await onlineBankingContext.ToListAsync());
        }

        // GET: Admin/Transactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .Include(t => t.Currency)
                .Include(t => t.FromAccount)
                .Include(t => t.ToAccount)
                .Include(t => t.FromAccount.AccountType)
                .Include(t => t.ToAccount.AccountType)
                .Include(t => t.FromAccount.User)
                .Include(t => t.ToAccount.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }


        private bool TransactionExists(int id)
        {
            return _context.Transactions.Any(e => e.Id == id);
        }
    }
}
