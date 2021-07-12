using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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

        [Route("PayyedDigibank/Transactions")]
        public async Task<IActionResult> Index()
        {
            var onlineBankingContext = _context.Transactions.Include(t => t.FromAccount).Include(t => t.ToAccount);
            ViewBag.AllTransactionByUser = _context.Transactions.Include(tra=>tra.FromAccount.User).Include(tra=>tra.ToAccount.User).Include(tra => tra.FromAccount).Include(tra=>tra.Currency).Include(tra => tra.ToAccount).Where(tra => tra.FromAccount.UserId == HttpContext.Session.GetInt32("IdCurrentUser") || tra.ToAccount.UserId == HttpContext.Session.GetInt32("IdCurrentUser"));

            return View(await onlineBankingContext.ToListAsync());
        }

        public IActionResult Send()
        {
            // truyên addressbook vao view
            ViewData["FromAccountId"] = new SelectList(_context.Accounts, "Id", "Number");
            ViewData["ToAccountId"] = new SelectList(_context.Accounts, "Id", "Number");
            return View();
        }
        public IActionResult Deposit()
        {
            // truyên addressbook vao view
            ViewData["AccountList"] = new SelectList(_context.Accounts.Where(acc => acc.UserId == HttpContext.Session.GetInt32("IdCurrentUser")), "Id", "Number");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DepositConfirm(int depositAmount, int AccountList)
        {
            int id = Convert.ToInt32(HttpContext.Session.GetInt32("IdCurrentUser"));
            ViewData["Amount"] = depositAmount;
            ViewData["Account"] = _context.Accounts.Where(a => a.Id == AccountList).Include(a => a.User).FirstOrDefault();
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> DepositSuccess(int Amount, int ToAccount, int Balance)
        {
            ViewData["Amount"] = Amount;
            var account = new Account() { Id = ToAccount, Balance = Balance };
            account.Balance += Amount;
            _context.Accounts.Attach(account);
            _context.Entry(account).Property(x => x.Balance).IsModified = true;
            await _context.SaveChangesAsync();

            Notification noti = new Notification();
            noti.UserId = HttpContext.Session.GetInt32("IdCurrentUser");
            noti.Message = "Deposit";
            noti.CreateDate = DateTime.Now;
            _context.Notifications.Add(noti);
            await _context.SaveChangesAsync();

            return View();
        }
        public IActionResult Withdraw()
        {
            // truyên addressbook vao view
            ViewData["FromAccountId"] = new SelectList(_context.Accounts, "Id", "Number");
            ViewData["ToAccountId"] = new SelectList(_context.Accounts, "Id", "Number");
            return View();
        }

        public IActionResult Request()
        {
            //Truyen vao list account của user
            return View();
        }

        // GET: UserSection/Transactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
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

            var transaction = await _context.Transactions.FindAsync(id);
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

            var transaction = await _context.Transactions
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
            var transaction = await _context.Transactions.FindAsync(id);
            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransactionExists(int id)
        {
            return _context.Transactions.Any(e => e.Id == id);
        }

        //Index nhưng là nước uống, vì có lọc :))
        [HttpPost]
        [Route("PayyedDigibank/Transactions")]
        public IActionResult Index(DateTime BeginDay, DateTime EndDay, string Filter, int NumberOf)
        {
            //Xử lý lại ngày kết thúc
            DateTime def = new DateTime();
            if (DateTime.Compare(EndDay, def) == 0)
            {
                EndDay = DateTime.Now;
            }
            int IdCurrentUser = (int)HttpContext.Session.GetInt32("IdCurrentUser");
            //Xử lý lại số transaction gần nhất người dùng muốn thấy
            if (NumberOf == 0)
            {
                NumberOf = _context.Transactions.Include(tra => tra.FromAccount.User).Include(tra => tra.ToAccount.User).Where(tra => tra.FromAccount.UserId == IdCurrentUser || tra.ToAccount.UserId == IdCurrentUser).Count();
            }

            
            if(Filter == "All")
            {
                ViewBag.AllTransactionByUser = _context.Transactions.Include(tra => tra.FromAccount.User).Include(tra => tra.ToAccount.User).Include(tra => tra.FromAccount).Include(tra => tra.Currency).Include(tra => tra.ToAccount).Where(tra => (tra.FromAccount.UserId == IdCurrentUser || tra.ToAccount.UserId == IdCurrentUser) && tra.IssuedDate >= BeginDay && tra.IssuedDate <= EndDay).OrderBy(tra=>tra.IssuedDate).Take(NumberOf);
                return View();
            }
            if(Filter == "OnlySender")
            {
                ViewBag.AllTransactionByUser = _context.Transactions.Include(tra => tra.FromAccount.User).Include(tra => tra.ToAccount.User).Include(tra => tra.FromAccount).Include(tra => tra.Currency).Include(tra => tra.ToAccount).Where(tra => (tra.FromAccount.UserId == IdCurrentUser) && tra.IssuedDate >= BeginDay && tra.IssuedDate <= EndDay).OrderBy(tra => tra.IssuedDate).Take(NumberOf);
                return View();
            }
            if(Filter == "OnlyRecipient")
            {
                ViewBag.AllTransactionByUser = _context.Transactions.Include(tra => tra.FromAccount.User).Include(tra => tra.ToAccount.User).Include(tra => tra.FromAccount).Include(tra => tra.Currency).Include(tra => tra.ToAccount).Where(tra => (tra.ToAccount.UserId == IdCurrentUser) && tra.IssuedDate >= BeginDay && tra.IssuedDate <= EndDay).OrderBy(tra => tra.IssuedDate).Take(NumberOf);
                return View();
            }

            var onlineBankingContext = _context.Transactions.Include(t => t.FromAccount).Include(t => t.ToAccount);
            return View(onlineBankingContext.ToList());
        }
    }
}
