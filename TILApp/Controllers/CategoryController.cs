using TILApp.Data;

namespace TILApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly Context db;

        public CategoryController(Context context)
        {
            db = context;
        }

        // GET: api/Category
        [HttpGet]
        public async Task<ActionResult<List<CategoryDto>>> GetCategory() =>
            db.Category.Any()
                ? Ok(await db.Category.Select(c => c.ToDto()).ToListAsync())
                : NotFound();

        // GET: api/Category/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CategoryDto>> GetCategory(int id)
        {
            var category = await db.Category.FindAsync(id);
            if (category == null) return NotFound();
            return category.ToDto();
        }

        // GET: api/Category/5/Acronyms
        [HttpGet("{id:int}/Acronyms")]
        public async Task<ActionResult<IEnumerable<AcronymDto>>> GetAcronym(int id)
        {
            var category = await db.Category
                .Where(i => i.Id == id)
                .Include(i => i.Acronyms)
                .FirstOrDefaultAsync();

            if (category == null || category.Acronyms == null) return NotFound();

            return category.Acronyms.Select(a => a.ToDto()).OrderBy(i => i.Id).ToList();
        }

        // PUT: api/Category/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id:int}"), Authorize]
        public async Task<IActionResult> PutCategory(int id, CategoryDto dto)
        {
            var category = await db.Category.Where(i => i.Id == id).FirstAsync();

            category.Id = dto.Id;
            category.Name = dto.Name;

            db.Category.Update(category);

            try { await db.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id)) return NotFound();
                else throw;
            }

            return Ok(category.ToDto());
        }

        // POST: api/Category
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost, Authorize]
        public async Task<ActionResult<Category>> PostCategory(CategoryDto dto)
        {
            var category = new Category() { Name = dto.Name };

            db.Category.Add(category);
            await db.SaveChangesAsync();

            return CreatedAtAction("GetCategory", new { id = category.Id }, category);
        }

        // DELETE: api/Category/5
        [HttpDelete("{id:int}"), Authorize]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await db.Category.FindAsync(id);

            if (category == null) return NotFound();

            db.Category.Remove(category);
            await db.SaveChangesAsync();

            return NoContent();
        }

        

        private bool CategoryExists(int id)
        {
            return (db.Category?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
