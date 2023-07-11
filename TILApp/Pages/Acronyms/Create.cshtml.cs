using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TILApp.Models;

namespace TILApp.Pages.Acronyms
{
    public class CreateModel : PageModel
    {
        private readonly AcronymContext _context;

        public CreateModel(AcronymContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["UserName"] = new SelectList(_context.User, "Name", "Name");
            return Page();
        }

        [BindProperty]
        public Acronym Acronym { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.Acronym == null || Acronym == null)
            {
                return Page();
            }

            _context.Acronym.Add(Acronym);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
