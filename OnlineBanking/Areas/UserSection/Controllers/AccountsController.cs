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
    public class AccountsController : Controller
    {
        private readonly OnlineBankingContext _context;

        public AccountsController(OnlineBankingContext context)
        {
            _context = context;
        }

        [Route("PayyedDigibank/User/Accounts")]
        public IActionResult Index()
        {
            int userid = Convert.ToInt32(HttpContext.Session.GetInt32("IdCurrentUser"));
            ViewData["AccountList"] = _context.Accounts.Include(a => a.AccountType).Include(a => a.User).Where(a => a.UserId == userid).ToList();
            ViewData["AccountTypeId"] = new SelectList(_context.AccountTypes, "Id", "Name");
            ViewData["AccountNumber"] = _context.Accounts.Select(a => a.Number).ToList();
            return View();

        }

        // POST: UserSection/Accounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AccountTypeId,Number,")] Account account)
        {
            if (ModelState.IsValid)
            {
                account.UserId = Convert.ToInt32(HttpContext.Session.GetInt32("IdCurrentUser"));
                account.Balance = 0;
                account.CreateDate = DateTime.Now;
                account.Status = true;
                _context.Add(account);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountTypeId"] = new SelectList(_context.AccountTypes, "Id", "Name", account.AccountTypeId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Address", account.UserId);
            return View(account);
        }

        // GET: UserSection/Accounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            ViewData["AccountTypeId"] = new SelectList(_context.AccountTypes, "Id", "Name", account.AccountTypeId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Address", account.UserId);
            return View(account);
        }

        // POST: UserSection/Accounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,AccountTypeId,Number,Balance,CreateDate,Status")] Account account)
        {
            if (id != account.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(account);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account.Id))
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
            ViewData["AccountTypeId"] = new SelectList(_context.AccountTypes, "Id", "Name", account.AccountTypeId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Address", account.UserId);
            return View(account);
        }

        // GET: UserSection/Accounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // POST: UserSection/Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.Id == id);
        }
    }
}
