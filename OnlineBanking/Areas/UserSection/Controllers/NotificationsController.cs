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
    public class NotificationsController : Controller
    {
        private readonly OnlineBankingContext _context;

        public NotificationsController(OnlineBankingContext context)
        {
            _context = context;
        }

        [Route("PayyedDigibank/User/Notifications")]
        public async Task<IActionResult> Index()
        {
            ViewBag.Current = "Notification";
            var onlineBankingContext = _context.Notifications.Include(n => n.Transaction).Include(n => n.User);
            return View(await onlineBankingContext.ToListAsync());
        }

        // GET: UserSection/Notifications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notification = await _context.Notifications
                .Include(n => n.Transaction)
                .Include(n => n.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (notification == null)
            {
                return NotFound();
            }

            return View(notification);
        }

        // GET: UserSection/Notifications/Create
        public IActionResult Create()
        {
            ViewData["TransactionId"] = new SelectList(_context.Transactions, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Address");
            return View();
        }

        // POST: UserSection/Notifications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,TransactionId,Message,CreateDate,HaveRead")] Notification notification)
        {
            if (ModelState.IsValid)
            {
                _context.Add(notification);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TransactionId"] = new SelectList(_context.Transactions, "Id", "Id", notification.TransactionId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Address", notification.UserId);
            return View(notification);
        }

        // GET: UserSection/Notifications/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
            {
                return NotFound();
            }
            ViewData["TransactionId"] = new SelectList(_context.Transactions, "Id", "Id", notification.TransactionId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Address", notification.UserId);
            return View(notification);
        }

        // POST: UserSection/Notifications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,TransactionId,Message,CreateDate,HaveRead")] Notification notification)
        {
            if (id != notification.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(notification);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NotificationExists(notification.Id))
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
            ViewData["TransactionId"] = new SelectList(_context.Transactions, "Id", "Id", notification.TransactionId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Address", notification.UserId);
            return View(notification);
        }

        // GET: UserSection/Notifications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notification = await _context.Notifications
                .Include(n => n.Transaction)
                .Include(n => n.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (notification == null)
            {
                return NotFound();
            }

            return View(notification);
        }

        // POST: UserSection/Notifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NotificationExists(int id)
        {
            return _context.Notifications.Any(e => e.Id == id);
        }
    }
}
