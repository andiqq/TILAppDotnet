using TILApp.Data;

// ReSharper disable ConvertTypeCheckPatternToNullCheck

namespace TILApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AcronymController(Context db) : ControllerBase
    {
        // GET: api/Acronym
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AcronymDto>>> GetAcronym() =>
            db.Acronym.Any()
                ? Ok(await db.Acronym.Select(a => a.ToDto()).ToListAsync())
                : NotFound();

        // GET: api/Acronym/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<AcronymDto>> GetAcronym(int id) =>
            await db.Acronym.FindAsync(id) is { } acronym
                ? Ok(acronym.ToDto())
                : NotFound();

        // GET: api/Acronym/5/Categories
        [HttpGet("{id:int}/Categories")]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories(int id) =>
            await db.Acronym.AsNoTracking()
                    .Include(a => a.Categories)
                    .FirstOrDefaultAsync(a => a.Id == id)
                is { Categories.Count: not 0 } acronym
                ? Ok(acronym.Categories.Select(c => c.ToDto()).ToList())
                : NotFound();

        // GET: api/Acronym/5/User
        [HttpGet("{id:int}/User")]
        public async Task<ActionResult<User.Public>> GetAcronymUser(int id) =>
            await db.Acronym.AsNoTracking()
                    .Include(a => a.User)
                    .FirstOrDefaultAsync(a => a.Id == id)
                is { User: not null } acronym
                ? new User.Public(acronym.User)
                : NotFound();

        // GET: api/Acronym/search?Term=OMG
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<AcronymDto>>> SearchAcronym(string term) => 
                await db.Acronym
                    .Where(a => a.Short == term || a.Long == term)
                    .Select(a => a.ToDto())
                    .ToListAsync();

        // GET: api/Acronym/first
        [HttpGet("first")]
        public async Task<ActionResult<AcronymDto>> FindFirstAcronym() => 
            (await db.Acronym.FirstAsync()).ToDto();
        
        // GET: api/Acronym/sort
        [HttpGet("sort")]
        public async Task<ActionResult<IEnumerable<AcronymDto>>> SortAcronym() => 
            await db.Acronym.OrderBy(a => a.Short)
                .Select(a => a.ToDto())
                .ToListAsync();

        // PUT: api/Acronym/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> PutAcronym(int id, AcronymDto dto)
        {
            var acronym = await db.Acronym.Where(i => i.Id == id).FirstAsync();

            if (acronym == null) return BadRequest();

            acronym.Long = dto.Long;
            acronym.Short = dto.Short;
            acronym.UserId = dto.UserId;

            // _context.Entry(acronym).State = EntityState.Modified;
            db.Acronym.Update(acronym);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AcronymExists(id)) return NotFound();
                else throw;
            }
            return Ok(acronym.ToDto());
        }

        // PUT: api/Acronym/5/User
        [HttpPut("{id}/User"), Authorize]
        public async Task<IActionResult> UpdateUser(int id, string? userid)
        {
            var acronym = await db.Acronym.Where(i => i.Id == id).FirstAsync();
            if (acronym == null) return BadRequest();
            var user = await db.User.Where(i => i.Id == userid).FirstAsync();
            if (user == null) return BadRequest();
            acronym.UserId = userid;

            db.Acronym.Update(acronym);
            await db.SaveChangesAsync();

            return Ok(acronym.ToDto());
        }

        // PUT: api/Acronym/5/Category
        [HttpPut("{id:int}/Category"), Authorize]
        public async Task<IActionResult> AddCategory(int id, int catid)
        {
            var acronym = await db.Acronym.Where(i => i.Id == id).Include(a => a.Categories).FirstAsync();
            if (acronym == null) return BadRequest();
            var category = await db.Category.Where(i => i.Id == catid).FirstAsync();
            if (category == null) return BadRequest();

            //_ = acronym.Categories.Append(category);
            acronym.Categories.Add(category);
            db.Acronym.Update(acronym);
            await db.SaveChangesAsync();

            return Ok(acronym.ToDto());
        }

        // POST: api/Acronym
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost, Authorize]
        public async Task<ActionResult<Acronym>> PostAcronym(AcronymDto dto)
        {
           var userName = HttpContext.User.Identity.Name;
           var userId = await db.User.AsNoTracking().Where(u => u.UserName == userName).Select(u => u.Id).FirstAsync();
            
            if (db.Acronym == null) return Problem("Entity set 'AcronymContext.Acronym'  is null.");

            var acronym = new Acronym() { Long = dto.Long, Short = dto.Short, UserId = userId };

            db.Acronym.Add(acronym);
            await db.SaveChangesAsync();
            
            return CreatedAtAction("GetAcronym", new { id = acronym.Id }, acronym);
        }

        // DELETE: api/Acronym/5
        [HttpDelete("{id:int}"), Authorize]
        public async Task<IActionResult> DeleteAcronym(int id)
        {
            if (db.Acronym == null) return NotFound();

            var acronym = await db.Acronym.FindAsync(id);

            if (acronym == null) return NotFound();

            db.Acronym.Remove(acronym);
            await db.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Acronym/5/Category
        [HttpDelete("{id}/Category"), Authorize]
        public async Task<IActionResult> DeleteCategory(int id, int catid)
        {
            var acronym = await db.Acronym.Where(i => i.Id == id).Include(a => a.Categories).FirstAsync();

            if (acronym == null) return BadRequest();

            var category = await db.Category.Where(i => i.Id == catid).FirstAsync();

            if (category == null || acronym.Categories == null) return BadRequest();

            acronym.Categories.Remove(category);

            db.Acronym.Update(acronym);
            await db.SaveChangesAsync();

            return NoContent();
        }

        private bool AcronymExists(int id)
        {
            return (db.Acronym?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}