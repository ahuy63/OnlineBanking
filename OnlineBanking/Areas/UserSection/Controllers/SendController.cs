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
    public class SendController : Controller
    {
        private readonly OnlineBankingContext _context;

        public SendController(OnlineBankingContext context)
        {
            _context = context;
        }

        //Đổi Router
        [Route("PayyedDigibank/Send")]
        public ActionResult Index()
        {
            if(HttpContext.Session.GetInt32("IdCurrentUser") == null)
            {
                return RedirectToAction("Login","Users",new { area = "" });
            }

            //Truyền danh sách Tiền tệ qua bên kia để đổi tiền, nhìn cho đẹp, vcl!!!!!!!
            ViewBag.lstCurrencies = _context.Currencies.ToList();

            //Lấy thông tin của User đem qua bên kia
            ViewBag.CurrentUser = _context.Users.Where(us => us.Id == HttpContext.Session.GetInt32("IdCurrentUser")).FirstOrDefault();

            //Lấy danh sách người nhận đem qua bên view
            ViewBag.AllAddressBookById = _context.AddressBooks.Include(ad => ad.Account).Include(ad => ad.Account.User).Include(ad => ad.User).Where(ad => ad.UserId == HttpContext.Session.GetInt32("IdCurrentUser"));

            //Lấy 1 người nhận default
            ViewBag.AccountRecipientDefault = _context.Accounts.Where(acc => acc.UserId == HttpContext.Session.GetInt32("IdCurrentUser")).FirstOrDefault().Number;

            //Lấy danh sách tài khoản đem qua bên view
            ViewBag.AccountList = _context.Accounts.Include(acc => acc.User).Include(acc =>acc.AccountType).Where(acc => acc.UserId == HttpContext.Session.GetInt32("IdCurrentUser"));

            //Tạo 1 account default
            ViewBag.AccountDefault = _context.Accounts.Include(acc => acc.User).Include(acc => acc.AccountType).Where(acc => acc.UserId == HttpContext.Session.GetInt32("IdCurrentUser")).ToList().FirstOrDefault();

            return View();
        }


        [HttpPost]
        public ActionResult SendConfirm(string SenderCurrency,double AmountSend, string AccountGet, string AccountFrom)
        {
            //Lấy thông tin tài khoản nhận
            ViewBag.Recipient = _context.Accounts.Include(acc => acc.User).Where(acc => acc.Number == AccountGet).ToList().FirstOrDefault();

            //Lấy thông tin tài khoản gửi
            ViewBag.AccountFrom = AccountFrom;

            //Lấy số tiền
            ViewBag.Amount = AmountSend;

            //Lấy đơn vị tiền
            ViewBag.Currency = SenderCurrency;

            return View();
        }

        [HttpPost]
        public ActionResult SendSuccess(string AccountFrom, string AccountTo, double Amount, string Currency, string Description)
        {
            //Thêm dữ liệu transaction người gửi vào Db
            Transaction transaction = new Transaction();
            transaction.FromAccountId = _context.Accounts.Where(acc => acc.Number == AccountFrom).FirstOrDefault().Id;
            transaction.ToAccountId = _context.Accounts.Where(acc => acc.Number == AccountTo).FirstOrDefault().Id;
            transaction.Amount = Amount;
            transaction.CurrencyId = _context.Currencies.Where(cu => cu.Name == Currency).FirstOrDefault().Id;
            transaction.IssuedDate = DateTime.Now;
            transaction.Status = true;
            //Vấn đề phát sinh ở đây này, vì New balance đang tính theo USD nên phải đổi lại, mà tại sao có cái newbalacne ở đây ????
            //À cái newBalance để dùng trong notification
            //Để dễ thì tính tỷ giá trước
            double tempAmount = Amount * _context.Currencies.Where(cu => cu.Name == Currency).FirstOrDefault().ExchangeRate;
            transaction.NewBalanceSender = _context.Accounts.Where(acc => acc.Number == AccountFrom).FirstOrDefault().Balance - tempAmount;
            transaction.NewBalanceRecipient = _context.Accounts.Where(acc => acc.Number == AccountTo).FirstOrDefault().Balance + tempAmount;
            _context.Transactions.Add(transaction);


            //Thay đổi Balance trong Account người nhận
            Account tempAccount = _context.Accounts.Where(acc => acc.Number == AccountFrom).FirstOrDefault();
            tempAccount.Balance = transaction.NewBalanceSender;
            _context.Accounts.Update(tempAccount);

            //Thay đổi Balance trong Account người gửi
            tempAccount = _context.Accounts.Where(acc => acc.Number == AccountTo).FirstOrDefault();
            tempAccount.Balance = transaction.NewBalanceRecipient;

            _context.SaveChanges();
            //Hoàn tất cập nhật Db

            //Lưu lại dữ liệu vào ViewBag và đưa qua View
            ViewBag.Amount = Amount;
            ViewBag.Currency = Currency;
            ViewBag.RecipientNumber = AccountTo;
            ViewBag.RecipientName = _context.Accounts.Include(acc => acc.User).Where(acc => acc.Number == AccountTo).FirstOrDefault().User.FullName;

            return View();
        }

        // GET: UserSection/Send/Details/5
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

        // GET: UserSection/Send/Create
        public IActionResult Create()
        {
            ViewData["FromAccountId"] = new SelectList(_context.Accounts, "Id", "Number");
            ViewData["ToAccountId"] = new SelectList(_context.Accounts, "Id", "Number");
            return View();
        }

        // POST: UserSection/Send/Create
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

        // GET: UserSection/Send/Edit/5
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

        // POST: UserSection/Send/Edit/5
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

        // GET: UserSection/Send/Delete/5
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

        // POST: UserSection/Send/Delete/5
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
    }
}
