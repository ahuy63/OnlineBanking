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

namespace OnlineBanking.Controllers
{
    public class UsersController : Controller
    {
        private readonly OnlineBankingContext _context;

        private static User UserIsLogining = new User();
        private static Int32 NumberCode = 0;

        public UsersController(OnlineBankingContext context)
        {
            _context = context;
        }

        
        //Hiển thi trang đăng nhập
        public IActionResult Login()
        {
            //Code Login
            return View();
        }

        //Xử lý đăng nhập
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string username, string password)
        {
            bool resultLogin = _context.Users.Any(us => us.Username == username && us.Password == password);
            if (resultLogin)
            {

                //Sử dụng một biến session để lưu Id người dùng hiện tại, đăng xuất thì mới xóa đi
                HttpContext.Session.SetInt32("IdCurrentUser", _context.Users.Where(us => us.Username == username).FirstOrDefault().Id);

                //Lưu lại tên người dùng để đưa ra Layout View
                HttpContext.Session.SetString("NameCurrentUser", _context.Users.Where(us => us.Username == username).FirstOrDefault().FullName);
                return RedirectToAction("Index","Home");
            }


            //Nếu không đăng nhập thành công, cài đặt một thông báo lỗi trả qua ViewBag
            ViewBag.MessLogin = "!Login Fail, please check again";
            return View();
        }

        // GET: Users/Create
        public IActionResult SignUp()
        {
            //Code SignUp
            return View();
        }

        // POST: Users/SignUp
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp([Bind("Id,Username,Password,FullName,NumberPhone,Address,DateCreate,IdentityCard,Status")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }*/


        //Xử lý gửi email xác nhận
        [HttpPost]
        public IActionResult Validate(string Username, string Password, string NumberPhone, string EmailAddress, string Address, string IdentityCard, string FullName)
        {
            //Đầu tiên kiểm tra xem Username này có bị trùng ko
            if(_context.Users.Any(us => us.Username == Username))
            {
                ViewBag.MessSignUp = "Have already this Username, please Choose another Username";
                return View("SignUp");
            }



            //Khi đăng nhập xong sẽ gửi một Email cho người nhận để xác minh cái email đó là real hay pha-ke
            //Gửi người dùng 1 cái mã xác minh rồi nhập vào


            //Tạo 1 mã ngẫu nhiên
            Random random = new Random();
            NumberCode = random.Next(1000, 99999);
            string message = "Your code is:" + NumberCode.ToString();

            //Gửi Email, phương thức gửi email nằm trong class EmailUser
            EmailUser emailUser = new EmailUser(EmailAddress, "Validate", message);
            emailUser.SendEmail(emailUser);


            //Đưa thông tin vào một biến UserIsLogining để đem qua hàm đăng ký
            UserIsLogining.Username = Username;
            UserIsLogining.Password = Password;
            UserIsLogining.FullName = FullName;
            UserIsLogining.Email = EmailAddress;
            UserIsLogining.Address = Address;
            UserIsLogining.NumberPhone = NumberPhone;
            UserIsLogining.IdentityCard = IdentityCard;
            UserIsLogining.IsAdmin = false;
            UserIsLogining.Status = true;
            

            return View("ValidateSignUp");
        }

        [HttpPost]
        public IActionResult SignUp(int Code)
        {
            if(Code == NumberCode)
            {
                ViewBag.MessLogin = "!You have Signed up was success, please login to continue";
                //Xử lý thêm user vào Db
                UserIsLogining.DateCreate = DateTime.Now;

                _context.Users.Add(UserIsLogining);
                _context.SaveChanges();
                return View("Login");
            }
            ViewBag.MessSignUp = "Wrong Code, please try another Code or Email";
            return View("SignUp");
        }
        
        public IActionResult SignOut()
        {
            HttpContext.Session.Remove("NameCurrentUser");
            HttpContext.Session.Remove("IdCurrentUser");
            return View("Login");
        }

       
    }
}
