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
using OnlineBanking.MyClass;

namespace OnlineBanking.Areas.UserSection.Controllers
{
    [Area("UserSection")]
    public class ChequesController : Controller
    {
        private readonly OnlineBankingContext _context;

        public ChequesController(OnlineBankingContext context)
        {
            _context = context;
        }


        [Route("PayyedDigibank/Cheques")]
        public IActionResult Index()
        {
            ViewBag.AllChequeByUser = _context.Cheques.Include(che=>che.Accounts.User).Include(che=>che.Accounts).Include(che=>che.currency).Where(che => che.Accounts.UserId == HttpContext.Session.GetInt32("IdCurrentUser"));
            return View();
        }

        //Index cheque nhưng là nước uống, vì có lọc ......
        [HttpPost]
        [Route("PayyedDigibank/Cheques")]
        public IActionResult Index(DateTime BeginDay, DateTime EndDay)
        {
            //Xử lý lại ngày kết thúc
            DateTime def = new DateTime();
            if (DateTime.Compare(EndDay, def) == 0)
            {
                EndDay = DateTime.Now;
            }

            List<Cheque> cheques = _context.Cheques.Include(che => che.Accounts.User).Include(che => che.Accounts).Include(che => che.currency).Where(che => che.Accounts.UserId == HttpContext.Session.GetInt32("IdCurrentUser") && che.DateCreate >= BeginDay && che.DateCreate <= EndDay).ToList();

            if (cheques.Count() >0) {
                ViewBag.AllChequeByUser = cheques;
            }
            else
            {
                ViewBag.AllChequeByUser = null;
            }
            return View();
        }



        [Route("PayyedDigibank/Request")]
        public ActionResult Making()
        {
            if (HttpContext.Session.GetInt32("IdCurrentUser") == null)
            {
                return RedirectToAction("Login", "Users", new { area = "" });
            }

            if (_context.Accounts.Where(acc => acc.UserId == HttpContext.Session.GetInt32("IdCurrentUser")).Count() == 0)
            {
                return RedirectToAction("Index", "Accounts");
            }
            ViewBag.ErrorOTP = TempData["ErrorOTP"];
            ViewBag.ErrorBalance = TempData["ErrorBalance"];

            //Lấy tất cả Account của người đó
            ViewBag.AccountList = _context.Accounts.Include(acc=>acc.AccountType).Where(acc => acc.UserId == HttpContext.Session.GetInt32("IdCurrentUser")).ToList();

            //Lấy thông tin của User đó
            ViewBag.CurrentUser = _context.Users.Where(us => us.Id == HttpContext.Session.GetInt32("IdCurrentUser")).FirstOrDefault();

            
            
            return View();
        }

        // GET: UserSection/Cheques/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cheque = await _context.Cheques
                .Include(c => c.Accounts)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cheque == null)
            {
                return NotFound();
            }

            return View(cheque);
        }

        // GET: UserSection/Cheques/Create
        public IActionResult Create()
        {
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Number");
            return View();
        }

        // POST: UserSection/Cheques/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AccountId,DateCreate,Address,Amount,Status")] Cheque cheque)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cheque);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Number", cheque.AccountId);
            return View(cheque);
        }

        // GET: UserSection/Cheques/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cheque = await _context.Cheques.FindAsync(id);
            if (cheque == null)
            {
                return NotFound();
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Number", cheque.AccountId);
            return View(cheque);
        }

        // POST: UserSection/Cheques/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AccountId,DateCreate,Address,Amount,Status")] Cheque cheque)
        {
            if (id != cheque.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cheque);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChequeExists(cheque.Id))
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
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Number", cheque.AccountId);
            return View(cheque);
        }

        // GET: UserSection/Cheques/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cheque = await _context.Cheques
                .Include(c => c.Accounts)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cheque == null)
            {
                return NotFound();
            }

            return View(cheque);
        }

        // POST: UserSection/Cheques/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cheque = await _context.Cheques.FindAsync(id);
            _context.Cheques.Remove(cheque);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChequeExists(int id)
        {
            return _context.Cheques.Any(e => e.Id == id);
        }


        //Action confirm của Cheque
        [HttpPost]
        public IActionResult ChequeConfirm(string AccountFrom, string AddressUser, double Amount, string SenderCurrency)
        {
            //Tính toán số tiền còn lại sau khi request
            double newBalance = _context.Accounts.Where(acc => acc.Number == AccountFrom).FirstOrDefault().Balance - Amount / (_context.Currencies.Where(curr => curr.Name == SenderCurrency).FirstOrDefault().ExchangeRate);
            if(newBalance <= 0)
            {
                TempData["ErrorBalance"] = "Your Balance is not Enough For This Request, please Try Again!!!!";
                return RedirectToAction("Making","Cheques");
            }

            
            //Cần phải validate bằng mail chứ ko thì mất tiền
            //Tạo 1 mã ngẫu nhiên
            Random random = new Random();
            int OTPNumber = random.Next(10000, 999999);
            HttpContext.Session.SetInt32("OTPSendMoney", OTPNumber);
            string message = "Your OTP Number is:" + OTPNumber.ToString();
            //Gửi Email
            EmailUser emailUser = new EmailUser(_context.Users.Where(use => use.Id == HttpContext.Session.GetInt32("IdCurrentUser")).FirstOrDefault().Email, "Send OTP", message);
            emailUser.SendEmail(emailUser);


            ViewBag.AccountFrom = AccountFrom;
            ViewBag.Amount = Amount;
            ViewBag.SenderCurrency = SenderCurrency;
            ViewBag.AddressUser = AddressUser;
            ViewBag.NewBalance = Math.Round(newBalance,2);

            return View();
        }





        [HttpPost]
        public IActionResult ChequeSuccess(string AccountFrom, double Amount, string SenderCurrency, string AddressUser, int OTP)
        {
            if(OTP != HttpContext.Session.GetInt32("OTPSendMoney"))
            {
                TempData["ErrorOTP"] = "You have input wrong OTP, please repeat proccess";
                return RedirectToAction("Making", "Cheques");
            }

            //Đã đúng OTP rồi thì lưu vào db
            Cheque cheque = new Cheque();
            cheque.AccountId = _context.Accounts.Where(acc =>acc.Number == AccountFrom).FirstOrDefault().Id;
            cheque.DateCreate = DateTime.Now;
            cheque.Amount = Amount;
            cheque.CurrencyId = _context.Currencies.Where(cur => cur.Name == SenderCurrency).FirstOrDefault().Id;
            cheque.Address = AddressUser;
            cheque.ProccessingStatus = false;
            cheque.Status = true;

            _context.Cheques.Add(cheque);
            //Thêm xong rồi thì trừ đi số tiền hiện có
            Account tempAccount = _context.Accounts.Where(acc => acc.Number == AccountFrom).FirstOrDefault();
            double newBalance = tempAccount.Balance - Amount / (_context.Currencies.Where(curr => curr.Name == SenderCurrency).FirstOrDefault().ExchangeRate);
            _context.Accounts.Update(tempAccount);


            _context.SaveChanges();

            //Lấy dữ liệu đưa qua view cho đẹp
            ViewBag.Amount = Amount;
            ViewBag.Currency = SenderCurrency;
            ViewBag.AccountFrom = AccountFrom;
            ViewBag.AddressUser = AddressUser;

            return View();
        }


        [HttpPost]
        public IActionResult UpdateAddress(int ChequeId, string NewAddress)
        {
            //Xử lý update
            Cheque cheque = _context.Cheques.Where(che => che.Id == ChequeId).FirstOrDefault();
            cheque.Address = NewAddress;
            _context.Cheques.Update(cheque);
            _context.SaveChanges();
            
            return RedirectToAction("Index");
        }
    }
}
