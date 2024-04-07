using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TILApp.Models;

namespace TILApp.Controllers
{
    public class AcronymViewController : Controller
    {
        private readonly AcronymContext _context;

        public AcronymViewController(AcronymContext context)
        {
            _context = context;
        }

        // GET: AcronymView
        public async Task<IActionResult> Index()
        {
            var acronymContext = _context.Acronym.Include(a => a.User);
            return View(await acronymContext.ToListAsync());
        }

        // GET: AcronymView/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var acronym = await _context.Acronym
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (acronym == null)
            {
                return NotFound();
            }

            return View(acronym);
        }

        // GET: AcronymView/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Id");
            return View();
        }

        // POST: AcronymView/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Short,Long,UserId")] Acronym acronym)
        {
            if (ModelState.IsValid)
            {
                _context.Add(acronym);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Id", acronym.UserId);
            return View(acronym);
        }

        // GET: AcronymView/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var acronym = await _context.Acronym.FindAsync(id);
            if (acronym == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Id", acronym.UserId);
            return View(acronym);
        }

        // POST: AcronymView/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Short,Long,UserId")] Acronym acronym)
        {
            if (id != acronym.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(acronym);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AcronymExists(acronym.Id))
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
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Id", acronym.UserId);
            return View(acronym);
        }

        // GET: AcronymView/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var acronym = await _context.Acronym
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (acronym == null)
            {
                return NotFound();
            }

            return View(acronym);
        }

        // POST: AcronymView/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var acronym = await _context.Acronym.FindAsync(id);
            if (acronym != null)
            {
                _context.Acronym.Remove(acronym);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AcronymExists(int id)
        {
            return _context.Acronym.Any(e => e.Id == id);
        }
    }
}
