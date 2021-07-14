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
    public class AccountsController : Controller
    {
        private readonly OnlineBankingContext _context;

        public AccountsController(OnlineBankingContext context)
        {
            _context = context;
        }

        // GET: Admin/Accounts
        public async Task<IActionResult> Index()
        {
            int id = Convert.ToInt32(HttpContext.Session.GetInt32("IdCurrentUser"));
            if (HttpContext.Session.GetString("NameCurrentUser") == null)
            {
                return RedirectToAction("Login", "Users");

            }
            var user = await _context.Users
            .FirstOrDefaultAsync(m => m.Id == id);
            if (!user.IsAdmin)
            {
                TempData["MessLogin"] = "Login with administrator privileges and try again";
                return RedirectToAction("Login", "Users");
            }
            var onlineBankingContext = _context.Accounts.Include(a => a.AccountType).Include(a => a.User);
            return View(await onlineBankingContext.ToListAsync());
        }

        // GET: Admin/Accounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .Include(a => a.AccountType)
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        public async Task<IActionResult> DeactivateConfirmed(int id, int userid)
        {
            var accounts = await _context.Accounts.FindAsync(id);
            accounts.Status = false;
            _context.Accounts.Attach(accounts);
            _context.Entry(accounts).Property(x => x.Status).IsModified = true;
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Users", new { id = userid});
        }

        public async Task<IActionResult> ActivateConfirmed(int id, int userid)
        {
            var accounts = await _context.Accounts.FindAsync(id);
            accounts.Status = true;
            _context.Accounts.Attach(accounts);
            _context.Entry(accounts).Property(x => x.Status).IsModified = true;
            await _context.SaveChangesAsync();
            return RedirectToAction( "Details", "Users", new { id = userid});
        }
        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.Id == id);
        }
    }
}
