using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TILApp.Models;

namespace TILApp.Pages.Acronyms
{
    public class EditModel : PageModel
    {
        private readonly AcronymContext _context;

        public EditModel(AcronymContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Acronym Acronym { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Acronym == null)
            {
                return NotFound();
            }

            var acronym =  await _context.Acronym.Include(a => a.Categories).FirstOrDefaultAsync(m => m.Id == id);
            if (acronym == null)
            {
                return NotFound();
            }
            Acronym = acronym;
           ViewData["UserId"] = new SelectList(_context.User, "Name", "Name");
            ViewData["CategoryId"] = new SelectList(_context.Category, "Name", "Name");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Acronym).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AcronymExists(Acronym.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool AcronymExists(int id)
        {
          return (_context.Acronym?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
