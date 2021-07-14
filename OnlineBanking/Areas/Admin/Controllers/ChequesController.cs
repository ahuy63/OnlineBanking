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
    public class ChequesController : Controller
    {
        private readonly OnlineBankingContext _context;

        public ChequesController(OnlineBankingContext context)
        {
            _context = context;
        }

        // GET: Admin/Cheques
        public async Task<IActionResult> Index()
        {
            var onlineBankingContext = _context.Cheques.Include(c => c.Accounts).Include(c => c.currency);
            return View(await onlineBankingContext.ToListAsync());
        }

        // GET: Admin/Cheques/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cheque = await _context.Cheques
                .Include(c => c.Accounts)
                .Include(c => c.currency)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cheque == null)
            {
                return NotFound();
            }

            return View(cheque);
        }
        public async Task<IActionResult> ChequeStatusAsync(int chequeid, string status)
        {
            int id = Convert.ToInt32(HttpContext.Session.GetInt32("IdCurrentUser"));
            var cheque = _context.Cheques.Where(c => c.Id == chequeid).FirstOrDefault();
            if (status == "false")
            {
                cheque.ProccessingStatus = false;
            }
            else
            {
                cheque.ProccessingStatus = true;
            }

            _context.Update(cheque);
            await _context.SaveChangesAsync();
            int accountId= Convert.ToInt32(_context.Cheques.Where(a => a.Id == chequeid).Select(a => a.AccountId).FirstOrDefault());
            double accountBalance = _context.Accounts.Where(a => a.Id == accountId).Select(a => a.Balance).FirstOrDefault();
            if (cheque.ProccessingStatus)
            {
                var account = new Account() { Id = accountId, Balance = accountBalance };
                account.Balance -= cheque.Amount;
                
                _context.Accounts.Attach(account);
                _context.Entry(account).Property(x => x.Balance).IsModified = true;
                await _context.SaveChangesAsync();


            }
            
            //Update account balance


            return RedirectToAction(nameof(Index));
        }

        private bool ChequeExists(int id)
        {
            return _context.Cheques.Any(e => e.Id == id);
        }
    }
}
