using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            if (HttpContext.Session.GetInt32("IdCurrentUser") == null)
            {
                return RedirectToAction("Login", "Users", new { area = "" });
            }

            //Nếu người dùng chưa có tài khoản ngân hàng nào thì đưa họ về trang đăng ký tài khoản ngân hàng
            if (_context.Accounts.Where(acc => acc.UserId == HttpContext.Session.GetInt32("IdCurrentUser")).Count() == 0)
            {
                return RedirectToAction("Index", "Accounts");
            }

            HttpContext.Session.SetInt32("PrepareToSend", 1);
            ViewBag.ErrorOTP = TempData["ErrorOTP"];


            //Truyền danh sách Tiền tệ qua bên kia để đổi tiền, nhìn cho đẹp, vcl!!!!!!!
            ViewBag.lstCurrencies = _context.Currencies.ToList();

            //Lấy thông tin của User đem qua bên kia
            ViewBag.CurrentUser = _context.Users.Where(us => us.Id == HttpContext.Session.GetInt32("IdCurrentUser")).FirstOrDefault();

            //Lấy danh sách người nhận đem qua bên view
            ViewBag.AllAddressBookById = _context.AddressBooks.Include(ad => ad.Account).Include(ad => ad.Account.User).Include(ad => ad.User).Where(ad => ad.UserId == HttpContext.Session.GetInt32("IdCurrentUser")).ToList();

            //Đếm số address book
            ViewBag.CountAddressBook = _context.AddressBooks.Where(ad => ad.UserId == HttpContext.Session.GetInt32("IdCurrentUser")).ToList().Count();

            //Lấy 1 người nhận default
            if (_context.AddressBooks.Where(ad => ad.UserId == HttpContext.Session.GetInt32("IdCurrentUser")).ToList().Count() != 0)
            {
                ViewBag.AccountRecipientDefault = _context.AddressBooks.Include(add => add.Account).Where(add => add.UserId == HttpContext.Session.GetInt32("IdCurrentUser")).FirstOrDefault().Account.Number;
            }

            //Lấy toàn bộ tài khoản
            ViewBag.AllAccountNumber = _context.Accounts.Select(acc => acc.Number).ToList();

            //Lấy danh sách tài khoản đem qua bên view
            ViewBag.AccountList = _context.Accounts.Include(acc => acc.User).Include(acc => acc.AccountType).Where(acc => acc.UserId == HttpContext.Session.GetInt32("IdCurrentUser"));

            //Tạo 1 account default
            ViewBag.AccountDefault = _context.Accounts.Include(acc => acc.User).Include(acc => acc.AccountType).Where(acc => acc.UserId == HttpContext.Session.GetInt32("IdCurrentUser")).ToList().FirstOrDefault();


            return View();
        }


        [HttpPost]
        public ActionResult SendConfirm(string SenderCurrency, double AmountSend, string AccountGet, string AccountFrom)
        {
            //Kiểm tra Xem người dùng nhập đúng số không, nếu sai trả về lại và gửi thông báo
            if(_context.Accounts.Include(acc => acc.User).Where(acc => acc.Number == AccountGet).ToList().FirstOrDefault() == null)
            {
                //Vì bên kia có 1 đống View bag thế là phải copy vào, nhìn rối vcl cơ mà chịu đó
                //Truyền danh sách Tiền tệ qua bên kia để đổi tiền, nhìn cho đẹp, vcl!!!!!!!
                ViewBag.lstCurrencies = _context.Currencies.ToList();

                //Lấy thông tin của User đem qua bên kia
                ViewBag.CurrentUser = _context.Users.Where(us => us.Id == HttpContext.Session.GetInt32("IdCurrentUser")).FirstOrDefault();

                //Lấy danh sách người nhận đem qua bên view
                ViewBag.AllAddressBookById = _context.AddressBooks.Include(ad => ad.Account).Include(ad => ad.Account.User).Include(ad => ad.User).Where(ad => ad.UserId == HttpContext.Session.GetInt32("IdCurrentUser")).ToList();

                //Đếm số address book
                ViewBag.CountAddressBook = _context.AddressBooks.Where(ad => ad.UserId == HttpContext.Session.GetInt32("IdCurrentUser")).ToList().Count();

                //Lấy 1 người nhận default
                ViewBag.AccountRecipientDefault = AccountGet;

                //Lấy toàn bộ tài khoản
                ViewBag.AllAccountNumber = _context.Accounts.Select(acc => acc.Number).ToList();

                //Lấy danh sách tài khoản đem qua bên view
                ViewBag.AccountList = _context.Accounts.Include(acc => acc.User).Include(acc => acc.AccountType).Where(acc => acc.UserId == HttpContext.Session.GetInt32("IdCurrentUser"));

                //Tạo 1 account default
                ViewBag.AccountDefault = _context.Accounts.Include(acc => acc.User).Include(acc => acc.AccountType).Where(acc => acc.UserId == HttpContext.Session.GetInt32("IdCurrentUser")).ToList().FirstOrDefault();
                //Rồi thêm xong, giờ phải cho nó cái thông báo
                ViewBag.ErrorRecipientAccount = "The Recipient's Account Was Wrong, please check again";


                return View("Index");
            }

            //Kiểm tra xem người dùng có đủ tiền mà chuyển không, không đủ thì chửi nó nghèo
            //Lấy số tiền gửi theo USD
            double AmountTemp = AmountSend / _context.Currencies.Where(cur => cur.Name == SenderCurrency).FirstOrDefault().ExchangeRate;

            //Kiểm tra và chửi nó
            if(_context.Accounts.Where(acc => acc.Number == AccountFrom).FirstOrDefault().Balance < AmountTemp)
            {
                //Vì bên kia có 1 đống View bag thế là phải copy vào, nhìn rối vcl cơ mà chịu đó
                //Truyền danh sách Tiền tệ qua bên kia để đổi tiền, nhìn cho đẹp, vcl!!!!!!!
                ViewBag.lstCurrencies = _context.Currencies.ToList();

                //Lấy thông tin của User đem qua bên kia
                ViewBag.CurrentUser = _context.Users.Where(us => us.Id == HttpContext.Session.GetInt32("IdCurrentUser")).FirstOrDefault();

                //Lấy danh sách người nhận đem qua bên view
                ViewBag.AllAddressBookById = _context.AddressBooks.Include(ad => ad.Account).Include(ad => ad.Account.User).Include(ad => ad.User).Where(ad => ad.UserId == HttpContext.Session.GetInt32("IdCurrentUser")).ToList();

                //Đếm số address book
                ViewBag.CountAddressBook = _context.AddressBooks.Where(ad => ad.UserId == HttpContext.Session.GetInt32("IdCurrentUser")).ToList().Count();

                //Lấy 1 người nhận default
                ViewBag.AccountRecipientDefault = AccountGet;

                //Lấy toàn bộ tài khoản
                ViewBag.AllAccountNumber = _context.Accounts.Select(acc => acc.Number).ToList();

                //Lấy danh sách tài khoản đem qua bên view
                ViewBag.AccountList = _context.Accounts.Include(acc => acc.User).Include(acc => acc.AccountType).Where(acc => acc.UserId == HttpContext.Session.GetInt32("IdCurrentUser"));

                //Tạo 1 account default
                ViewBag.AccountDefault = _context.Accounts.Include(acc => acc.User).Include(acc => acc.AccountType).Where(acc => acc.UserId == HttpContext.Session.GetInt32("IdCurrentUser")).ToList().FirstOrDefault();
                //Rồi thêm xong, giờ phải cho nó cái thông báo
                ViewBag.ErrorBalance = "You don't have Enough money!!!!!";


                return View("Index");
            }




            //Qua đến đây là gửi thành công
            //Lấy thông tin tài khoản nhận
            ViewBag.Recipient = _context.Accounts.Include(acc => acc.User).Where(acc => acc.Number == AccountGet).ToList().FirstOrDefault();

            //Lấy thông tin tài khoản gửi
            ViewBag.AccountFrom = AccountFrom;

            //Lấy số tiền
            ViewBag.Amount = AmountSend;

            //Lấy đơn vị tiền
            ViewBag.Currency = SenderCurrency;

            //Kiểm tra xem người nhận đó có trong addressbook của người gửi chưa
            ViewBag.IsNewAddressBook = !_context.AddressBooks.Any(add => add.AccountId == _context.Accounts.Where(acc => acc.Number == AccountGet).FirstOrDefault().Id && add.UserId == HttpContext.Session.GetInt32("IdCurrentUser"));

            //Cần phải validate bằng mail chứ ko thì mất tiền ăn l**
            //Tạo 1 mã ngẫu nhiên
            Random random = new Random();
            int OTPNumber = random.Next(10000, 999999);
            HttpContext.Session.SetInt32("OTPSendMoney", OTPNumber); 
            string message = "Your OTP Number is:" + OTPNumber.ToString();
            //Gửi Email
            EmailUser emailUser = new EmailUser(_context.Users.Where(use => use.Id  == HttpContext.Session.GetInt32("IdCurrentUser")).FirstOrDefault().Email, "Send OTP", message);
            emailUser.SendEmail(emailUser);


            return View();
        }

        [HttpPost]
        public ActionResult SendSuccess(string AccountFrom, string AccountTo, double Amount, string Currency, string Description, bool IsWantToSave, int OTP)
        {
            if(OTP != HttpContext.Session.GetInt32("OTPSendMoney"))
            {
                //Sai mã OTP, trả về Sendindex lại
                TempData["ErrorOTP"] = "You have input wrong OTP Code, please repeat the process";
                return RedirectToAction("Index", "Send");
            }

            if (HttpContext.Session.GetInt32("PrepareToSend") == 1)
            {
                //Nếu user muốn thêm người gửi thì add vào cho nó, không nó lại dỗi
                if (IsWantToSave)
                {
                    AddressBook addressBook = new AddressBook();
                    addressBook.UserId = HttpContext.Session.GetInt32("IdCurrentUser");
                    addressBook.AccountId = _context.Accounts.Where(acc => acc.Number == AccountTo).FirstOrDefault().Id;
                    _context.AddressBooks.Add(addressBook);
                }


                //Thêm dữ liệu transaction người gửi vào Db
                Transaction transaction = new Transaction();
                transaction.FromAccountId = _context.Accounts.Where(acc => acc.Number == AccountFrom).FirstOrDefault().Id;
                transaction.ToAccountId = _context.Accounts.Where(acc => acc.Number == AccountTo).FirstOrDefault().Id;
                transaction.Amount = Amount;
                transaction.CurrencyId = _context.Currencies.Where(cu => cu.Name == Currency).FirstOrDefault().Id;
                transaction.IssuedDate = DateTime.Now;
                transaction.Description = Description;
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
                //Hoàn tất cập nhật Db cho transaction
                //Cơ mà phải tạo thêm thông báo

                //Thông báo cho người gửi
                Notification notiSender = new Notification();
                notiSender.UserId = HttpContext.Session.GetInt32("IdCurrentUser");
                notiSender.TransactionId = _context.Transactions.OrderBy(tran => tran.Id).LastOrDefault().Id;
                notiSender.Message = "Send";
                notiSender.CreateDate = DateTime.Now;
                _context.Notifications.Add(notiSender);

                //Thông báo cho người nhận
                Notification notiRecipient = new Notification();
                notiRecipient.UserId = _context.Accounts.Where(acc => acc.Number == AccountTo).FirstOrDefault().UserId;
                notiRecipient.TransactionId = _context.Transactions.OrderBy(tran => tran.Id).LastOrDefault().Id;
                notiRecipient.Message = "Get";
                notiRecipient.CreateDate = DateTime.Now;
                _context.Notifications.Add(notiRecipient);
                _context.SaveChanges();
                //Ok cập nhật xong thông báo, thằng nào chế ra cái noti loz này vậy ?



                //Lưu lại dữ liệu vào ViewBag và đưa qua View
                ViewBag.Amount = Amount;
                ViewBag.Currency = Currency;
                ViewBag.RecipientNumber = AccountTo;
                ViewBag.RecipientName = _context.Accounts.Include(acc => acc.User).Where(acc => acc.Number == AccountTo).FirstOrDefault().User.FullName;

                HttpContext.Session.SetInt32("PrepareToSend", 0);
                return View();
            }
            else
            {
                ViewBag.Amount = Amount;
                ViewBag.Currency = Currency;
                ViewBag.RecipientNumber = AccountTo;
                ViewBag.RecipientName = _context.Accounts.Include(acc => acc.User).Where(acc => acc.Number == AccountTo).FirstOrDefault().User.FullName;
                return View();
            }
            
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
