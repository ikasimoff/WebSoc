using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Web.Data;
using Web.Models.DB;

namespace Web.Controllers
{
    [Authorize(Roles = "business")]
    public class BusinessController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BusinessController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Business
        public async Task<IActionResult> Index()
        {
            return View(await _context.Logins.ToListAsync());
        }

        // GET: Business/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var logins = await _context.Logins
                .FirstOrDefaultAsync(m => m.Id == id);
            if (logins == null)
            {
                return NotFound();
            }

            return View(logins);
        }

        // GET: Business/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Business/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Login,Password,AccountId,IsBlocked,LastUse,UseForParsing,UseProxy,ProxyIp,Description,LastUpdate")] Logins logins)
        {
            if (ModelState.IsValid)
            {
                _context.Add(logins);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(logins);
        }

        // GET: Business/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var logins = await _context.Logins.FindAsync(id);
            if (logins == null)
            {
                return NotFound();
            }
            return View(logins);
        }

        // POST: Business/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Login,Password,AccountId,IsBlocked,LastUse,UseForParsing,UseProxy,ProxyIp,Description,LastUpdate")] Logins logins)
        {
            if (id != logins.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(logins);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoginsExists(logins.Id))
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
            return View(logins);
        }

        // GET: Business/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var logins = await _context.Logins
                .FirstOrDefaultAsync(m => m.Id == id);
            if (logins == null)
            {
                return NotFound();
            }

            return View(logins);
        }

        // POST: Business/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var logins = await _context.Logins.FindAsync(id);
            _context.Logins.Remove(logins);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LoginsExists(long id)
        {
            return _context.Logins.Any(e => e.Id == id);
        }
    }
}
