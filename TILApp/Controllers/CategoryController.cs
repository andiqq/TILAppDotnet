using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TILApp.Models;

namespace TILApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly AcronymContext db;

        public CategoryController(AcronymContext context)
        {
            db = context;
        }

        // GET: api/Category
        [HttpGet]
        public async Task<ActionResult<ICollection<CategoryDto>>> GetCategory()
        {
            if (db.Category == null) return NotFound();
            return CategoryDto.convertedFrom(await db.Category.ToListAsync());
        }

        // GET: api/Category/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetCategory(int id)
        {
            if (db.Category == null) return NotFound();
            var category = await db.Category.FindAsync(id);
            if (category == null) return NotFound();
            return CategoryDto.convertedFrom(category);
        }

        // GET: api/Category/5/Acronyms
        [HttpGet("{id}/Acronyms")]
        public async Task<ActionResult<IEnumerable<AcronymDto>>> GetAcronym(int id)
        {
            if (db.Category == null) return NotFound();

            var category = await db.Category
                .Where(i => i.Id == id)
                .Include(i => i.Acronyms)
                .FirstOrDefaultAsync();

            if (category == null || category.Acronyms == null) return NotFound();

            return category.Acronyms.Select(a => AcronymDto.convertedFrom(a)).OrderBy(i => i.Id).ToList();
        }

        // PUT: api/Category/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, CategoryDto dto)
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

            return Ok(CategoryDto.convertedFrom(category));
        }

        // POST: api/Category
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory(CategoryDto dto)
        {
            if (db.Category == null) return Problem("Entity set 'AcronymContext.Category' is null.");

            var category = new Category() { Name = dto.Name };

            db.Category.Add(category);
            await db.SaveChangesAsync();

            return CreatedAtAction("GetCategory", new { id = dto.Id }, dto);
        }

        // DELETE: api/Category/5
        [HttpDelete("{id}")]
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
