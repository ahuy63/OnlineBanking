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

namespace OnlineBanking.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : Controller
    {
        private readonly OnlineBankingContext _context;

        public UsersController(OnlineBankingContext context)
        {
            _context = context;
        }

        // GET: Admin/Users
        public async Task<IActionResult> Index()
        {
            int id = Convert.ToInt32(HttpContext.Session.GetInt32("IdCurrentUser"));
            if (HttpContext.Session.GetString("NameCurrentUser") == null)
            {
                return RedirectToAction("Login", "Users");

            }
            var userLogin = await _context.Users
            .FirstOrDefaultAsync(m => m.Id == id);
            if (!userLogin.IsAdmin)
            {
                TempData["MessLogin"] = "Login with administrator privileges and try again";
                return RedirectToAction("Login", "Users");
            }

            ViewData["NoOfAccount"] = _context.Accounts.GroupBy(a => a.UserId).Count();

            var user = await _context.Users.ToListAsync();
            
            return View(user);
        }

        // GET: Admin/Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            ViewData["AccountList"] = _context.Accounts.Where(a => a.UserId == id).Include(a => a.AccountType).ToList();
            ViewData["Balance"] = _context.Accounts.Where(a => a.UserId == id).Sum(a => a.Balance);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }


        // POST: Admin/Users/Delete/5

        public async Task<IActionResult> DeactivateConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            user.Status = false;
            _context.Users.Attach(user);
            _context.Entry(user).Property(x => x.Status).IsModified = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ActivateConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            user.Status = true;
            _context.Users.Attach(user);
            _context.Entry(user).Property(x => x.Status).IsModified = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
