namespace TILApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AcronymController : ControllerBase
    {
        private readonly Context db;

        public AcronymController(Context context)
        {
            db = context;
        }

        // GET: api/Acronym
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Acronym.Dto>>> GetAcronym()
        {
            if (db.Acronym == null) return NotFound();
            
            return Ok(new Acronym.Dto().List(await db.Acronym.ToListAsync()));
        }

        // GET: api/Acronym/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Acronym.Dto>> GetAcronym(int id)
        {
            // if (db.Acronym == null) return NotFound();
            var acronym = await db.Acronym.FindAsync(id);
            return (acronym != null) ? Ok(new Acronym.Dto(acronym)) : NotFound();
        }

        // GET: api/Acronym/5/Categories
        [HttpGet("{id}/Categories")]
        public async Task<ActionResult<IEnumerable<Category.CDto>>> GetCategories(int id)
        {
            // if (db.Acronym == null) return NotFound();

            var acronym = await db.Acronym
                .Where(i => i.Id == id)
                .Include(i => i.Categories)
                .FirstOrDefaultAsync();
            return (acronym == null || acronym.Categories == null) ? NotFound() : Ok(new Category.CDto().List(acronym.Categories.ToList()));
        }

        // GET: api/Acronym/5/User
        [HttpGet("{id}/User")]
        public async Task<ActionResult<User.Public>> GetAcronymUser(int id)
        {
            if (db.Acronym == null) return NotFound();
            var acronym = await db.Acronym.FindAsync(id);
            if (acronym == null) return NotFound();
            var user = await db.User.FindAsync(acronym.UserId);
            if (user == null) return NotFound();
            return new User.Public(user);
        }

        // GET: api/Acronym/search?Term=OMG
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Acronym.Dto>>> SearchAcronym(string Term)
        { return new Acronym.Dto().List(await db.Acronym.Where(a => a.Short == Term || a.Long == Term).ToListAsync()); }

        // GET: api/Acronym/first
        [HttpGet("first")]
        public async Task<ActionResult<Acronym.Dto>> FindFirstAcronym()
        { return new Acronym.Dto(await db.Acronym.FirstAsync()); }

        // GET: api/Acronym/sort
        [HttpGet("sort")]
        public async Task<ActionResult<IEnumerable<Acronym.Dto>>> SortAcronym()
        { return new Acronym.Dto().List(await db.Acronym.OrderBy(a => a.Short).ToListAsync()); }

        // PUT: api/Acronym/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAcronym(int id, Acronym.Dto dto)
        {
            var acronym = await db.Acronym.Where(i => i.Id == id).FirstAsync();

            if (acronym == null) return BadRequest();

            acronym.Long = dto.Long;
            acronym.Short = dto.Short;
            acronym.UserId = dto.UserId;

            // _context.Entry(acronym).State = EntityState.Modified;
            db.Acronym.Update(acronym);

            try { await db.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException)
            {
                if (!AcronymExists(id)) return NotFound();
                else { throw; }
            }

            return Ok(new Acronym.Dto(acronym));
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

            return Ok(new Acronym.Dto(acronym));

        }

        // PUT: api/Acronym/5/Category
        [HttpPut("{id}/Category")]
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

            return Ok(new Acronym.Dto(acronym));
        }

        // POST: api/Acronym
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost, Authorize]
        public async Task<ActionResult<Acronym>> PostAcronym(Acronym.Dto dto)
        {
            if (db.Acronym == null) return Problem("Entity set 'AcronymContext.Acronym'  is null.");

            var acronym = new Acronym() { Long = dto.Long, Short = dto.Short, UserId = dto.UserId };

            db.Acronym.Add(acronym);
            await db.SaveChangesAsync();

           // return CreatedAtAction("GetAcronym", new { id = dto.Id }, dto);
           return CreatedAtAction("GetAcronym", new { id = acronym.Id }, acronym);
        }

        // DELETE: api/Acronym/5
        [HttpDelete("{id}"), Authorize]
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
