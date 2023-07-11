using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TILApp.Models;

namespace TILApp.Pages.Acronyms
{
    public class DetailsModel : PageModel
    {
        private readonly AcronymContext _context;

        public DetailsModel(AcronymContext context)
        {
            _context = context;
        }

      public Acronym Acronym { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Acronym == null) { return NotFound(); }
        
            var acronym = await _context.Acronym.Include(b => b.Categories).Include(a => a.User).FirstOrDefaultAsync(m => m.Id == id);

            if (acronym == null) { return NotFound(); }
            else { Acronym = acronym; }

            return Page();
        }
    }
}
