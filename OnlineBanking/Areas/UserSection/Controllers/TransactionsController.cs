using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineBanking.Data;
using OnlineBanking.Models;

namespace OnlineBanking.Areas.UserSection.Controllers
{
    [Area("UserSection")]
    public class TransactionsController : Controller
    {
        private readonly OnlineBankingContext _context;

        public TransactionsController(OnlineBankingContext context)
        {
            _context = context;
        }

        // GET: UserSection/Transactions
        public async Task<IActionResult> Index()
        {
            var onlineBankingContext = _context.Transaction.Include(t => t.FromAccount).Include(t => t.ToAccount);
            return View(await onlineBankingContext.ToListAsync());
        }

        // GET: UserSection/Transactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transaction
                .Include(t => t.FromAccount)
                .Include(t => t.ToAccount)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // GET: UserSection/Transactions/Create
        public IActionResult Create()
        {
            ViewData["FromAccountId"] = new SelectList(_context.Accounts, "Id", "Number");
            ViewData["ToAccountId"] = new SelectList(_context.Accounts, "Id", "Number");
            return View();
        }

        // POST: UserSection/Transactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FromAccountId,ToAccountId,Amount,NewBalance,IssuedDate,Status")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                _context.Add(transaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FromAccountId"] = new SelectList(_context.Accounts, "Id", "Number", transaction.FromAccountId);
            ViewData["ToAccountId"] = new SelectList(_context.Accounts, "Id", "Number", transaction.ToAccountId);
            return View(transaction);
        }

        // GET: UserSection/Transactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transaction.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }
            ViewData["FromAccountId"] = new SelectList(_context.Accounts, "Id", "Number", transaction.FromAccountId);
            ViewData["ToAccountId"] = new SelectList(_context.Accounts, "Id", "Number", transaction.ToAccountId);
            return View(transaction);
        }

        // POST: UserSection/Transactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FromAccountId,ToAccountId,Amount,NewBalance,IssuedDate,Status")] Transaction transaction)
        {
            if (id != transaction.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(transaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactionExists(transaction.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["FromAccountId"] = new SelectList(_context.Accounts, "Id", "Number", transaction.FromAccountId);
            ViewData["ToAccountId"] = new SelectList(_context.Accounts, "Id", "Number", transaction.ToAccountId);
            return View(transaction);
        }

        // GET: UserSection/Transactions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transaction
                .Include(t => t.FromAccount)
                .Include(t => t.ToAccount)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // POST: UserSection/Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transaction = await _context.Transaction.FindAsync(id);
            _context.Transaction.Remove(transaction);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransactionExists(int id)
        {
            return _context.Transaction.Any(e => e.Id == id);
        }
    }
}
