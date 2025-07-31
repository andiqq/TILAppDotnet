using TILApp.Data;

namespace TILApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController(Context db) : ControllerBase
    {
        // GET: api/Category
        [HttpGet]
        public async Task<ActionResult> GetCategory() =>
            Ok(await db.Category
                .AsNoTracking()
                .Select(c => c.ToDto())
                .ToListAsync());

        // GET: api/Category/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CategoryDto>> GetCategory(int id) =>
            await db.Category.FindAsync(id)
                is { } category
                ? Ok(category.ToDto())
                : NotFound();

        // GET: api/Category/5/Acronyms
        [HttpGet("{id:int}/Acronyms")]
        public async Task<ActionResult<IEnumerable<AcronymDto>>> GetAcronym(int id)
        {
            var category = await db.Category
                .Where(i => i.Id == id)
                .Include(i => i.Acronyms)
                .FirstOrDefaultAsync();

            if (category == null || category.Acronyms == null) return NotFound();

            return category.Acronyms.Select(a => a.ToDto()).OrderBy(i => i.Short).ToList();
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
        
        // Routes with authorization

        // PUT: api/Category/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id:int}"), Authorize]
        public async Task<IActionResult> PutCategory(int id, CategoryDto dto) =>
            await db.Category
                .Where(c => c.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(c => c.Id, dto.Id)
                    .SetProperty(c => c.Name, dto.Name)
                ) == 1
                ? Ok(dto)
                : NotFound();
        
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
