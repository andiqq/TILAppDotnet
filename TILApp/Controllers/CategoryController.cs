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
        public async Task<ActionResult<ICollection<Category.CDto>>> GetCategory()
        {
            if (db.Category == null) return NotFound();
            return new Category.CDto().List(await db.Category.ToListAsync());
        }

        // GET: api/Category/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Category.CDto>> GetCategory(int id)
        {
            if (db.Category == null) return NotFound();
            var category = await db.Category.FindAsync(id);
            if (category == null) return NotFound();
            return new Category.CDto(category);
        }

        // GET: api/Category/5/Acronyms
        [HttpGet("{id}/Acronyms")]
        public async Task<ActionResult<IEnumerable<Acronym.Dto>>> GetAcronym(int id)
        {
            if (db.Category == null) return NotFound();

            var category = await db.Category
                .Where(i => i.Id == id)
                .Include(i => i.Acronyms)
                .FirstOrDefaultAsync();

            if (category == null || category.Acronyms == null) return NotFound();

            return category.Acronyms.Select(a => new Acronym.Dto(a)).OrderBy(i => i.Id).ToList();
        }

        // PUT: api/Category/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> PutCategory(int id, Category.CDto dto)
        {
            var category = await db.Category.Where(i => i.Id == id).FirstAsync();

            if (category == null) return BadRequest();

            category.Id = dto.Id;
            category.Name = dto.Name;

            db.Category.Update(category);

            try { await db.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id)) return NotFound();
                else throw;
            }

            return Ok(new Category.CDto(category));
        }

        // POST: api/Category
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost, Authorize]
        public async Task<ActionResult<Category>> PostCategory(Category.CDto dto)
        {
            if (db.Category == null) return Problem("Entity set 'AcronymContext.Category' is null.");

            var category = new Category() { Name = dto.Name };

            db.Category.Add(category);
            await db.SaveChangesAsync();

            return CreatedAtAction("GetCategory", new { id = category.Id }, category);
        }

        // DELETE: api/Category/5
        [HttpDelete("{id}"), Authorize]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if (db.Category == null) return NotFound();

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
