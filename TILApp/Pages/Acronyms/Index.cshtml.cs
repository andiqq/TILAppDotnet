using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TILApp.Models;

namespace TILApp.Pages.Acronyms
{
    public class IndexModel : PageModel
    {
        private readonly AcronymContext _context;

        public IndexModel(AcronymContext context)
        {
            _context = context;
        }

        public IList<Acronym> Acronyms { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Acronym != null)
            {
                Acronyms = await _context.Acronym
                .Include(a => a.User).ToListAsync();
            }
        }
    }
}
