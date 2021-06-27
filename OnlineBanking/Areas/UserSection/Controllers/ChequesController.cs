using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineBanking.Data;
using OnlineBanking.Models;

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

        [Route("PayyedDigibank/Request")]
        public ActionResult Index()
        {
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
    }
}
