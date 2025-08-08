using System.Security.Claims;
using TILApp.Data;

namespace TILApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AcronymController(Context db) : ControllerBase
    {
        // GET: api/Acronym
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AcronymDto>>> GetAcronym() =>
            Ok(await db.Acronym
                .AsNoTracking()
                .Select(a => a.ToDto())
                .ToListAsync());

        // GET: api/Acronym/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<AcronymDto>> GetAcronym(int id) =>
            await db.Acronym.FindAsync(id) is { } acronym
                ? Ok(acronym.ToDto())
                : NotFound();

        // GET: api/Acronym/search?Term=TIL
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<AcronymDto>>> SearchAcronym(string term) =>
            await db.Acronym.AsNoTracking()
                .Where(a => a.Short == term || a.Long == term)
                .Select(a => a.ToDto())
                .ToListAsync();

        // GET: api/Acronym/first
        [HttpGet("first")]
        public async Task<ActionResult<AcronymDto>> FindFirstAcronym() =>
            await db.Acronym.AsNoTracking().FirstAsync() is { } acronym
                ? Ok(acronym.ToDto())
                : NotFound();

        // GET: api/Acronym/sort
        [HttpGet("sort")]
        public async Task<ActionResult<IEnumerable<AcronymDto>>> SortAcronym() =>
            await db.Acronym.AsNoTracking()
                .OrderBy(a => a.Short)
                .Select(a => a.ToDto())
                .ToListAsync();

        // GET: api/Acronym/5/User
        [HttpGet("{id:int}/User")]
        public async Task<ActionResult<User.Public>> GetAcronymUser(int id) =>
            await db.Acronym.AsNoTracking()
                    .Include(a => a.User)
                    .FirstOrDefaultAsync(a => a.Id == id)
                is { } acronym
                ? new User.Public(acronym.User)
                : NotFound();

        // GET: api/Acronym/5/Categories
        [HttpGet("{id:int}/Categories")]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories(int id) =>
            await db.Acronym.AsNoTracking()
                    .Include(a => a.Categories)
                    .FirstOrDefaultAsync(a => a.Id == id)
                is { } acronym
                ? Ok(acronym.Categories.Select(c => c.ToDto()))
                : NotFound();

        // Routes with authentication

        // POST: api/Acronym
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost, Authorize]
        public async Task<ActionResult<Acronym>> PostAcronym(AcronymDto dto)
        {
            var acronym = new Acronym
            {
                Long = dto.Long,
                Short = dto.Short,
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!
            };

            await db.Acronym.AddAsync(acronym);
            await db.SaveChangesAsync();
            
            return CreatedAtAction("GetAcronym", new { id = acronym.Id }, acronym.ToDto());
            
        }

        // PUT: api/Acronym/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> PutAcronym(int id, AcronymDto dto) =>
            await db.Acronym.Where(a => a.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(a => a.Long, dto.Long)
                    .SetProperty(a => a.Short, dto.Short)
                    .SetProperty(a => a.UserId, User.FindFirstValue(ClaimTypes.NameIdentifier))
                ) == 1
                ? Ok(dto)
                : NotFound();

        // PUT: api/Acronym/5/Category
        [HttpPut("{id:int}/Category"), Authorize]
        public async Task<IActionResult> AddCategory(int id, int catid)
        {
            var acronym = await db.Acronym.Include(a => a.Categories).FirstOrDefaultAsync(a => a.Id == id);

            if (acronym == null) return NotFound();

            var category = await db.Category.FindAsync(catid);

            if (category == null) return NotFound();

            acronym.Categories.Add(category);
            db.Acronym.Update(acronym);
            await db.SaveChangesAsync();
            
            return CreatedAtAction("GetCategories", new { id = acronym.Id }, category.ToDto());
        }

        // DELETE: api/Acronym/5
        [HttpDelete("{id:int}"), Authorize]
        public async Task<IActionResult> DeleteAcronym(int id)
            => await db.Acronym
                .Where(a => a.Id == id)
                .ExecuteDeleteAsync() == 1
                ? NoContent()
                : NotFound();

        // DELETE: api/Acronym/5/Category
        [HttpDelete("{id:int}/Category"), Authorize]
        public async Task<IActionResult> DeleteCategory(int id, int catid)
        {
            var acronym = await db.Acronym
                .Where(i => i.Id == id)
                .Include(a => a.Categories)
                .FirstOrDefaultAsync();
            
            if (acronym == null) return NotFound();

            var category = acronym.Categories.FirstOrDefault(c => c.Id == catid);

            if (category == null) return NotFound();
            
            acronym.Categories.Remove(category);

            db.Acronym.Update(acronym);
            await db.SaveChangesAsync();

            return NoContent();
        }
    }
}