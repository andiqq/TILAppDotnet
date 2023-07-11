using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TILApp.Models;

namespace TILApp.Pages.Acronyms
{
    public class DeleteModel : PageModel
    {
        private readonly AcronymContext _context;

        public DeleteModel(AcronymContext context)
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

            var acronym = await _context.Acronym.FirstOrDefaultAsync(m => m.Id == id);

            if (acronym == null)
            {
                return NotFound();
            }
            else 
            {
                Acronym = acronym;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Acronym == null)
            {
                return NotFound();
            }
            var acronym = await _context.Acronym.FindAsync(id);

            if (acronym != null)
            {
                Acronym = acronym;
                _context.Acronym.Remove(Acronym);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
