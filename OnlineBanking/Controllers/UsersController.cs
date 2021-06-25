using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineBanking.Data;
using OnlineBanking.Models;

namespace OnlineBanking.Controllers
{
    public class UsersController : Controller
    {
        private readonly OnlineBankingContext _context;

        public UsersController(OnlineBankingContext context)
        {
            _context = context;
        }

        //// GET: Users
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.Users.ToListAsync());
        //}

        public IActionResult Login()
        {
            //Code Login
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            //Code Login
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
        [HttpPost]
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
        }
    }
}
