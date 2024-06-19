namespace TILApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AcronymContext db;

        public UserController(AcronymContext context)
        {
            db = context;
        }

        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User.Public>>> GetUser()
        {
            if (db.User == null) return NotFound();

            return new User.Public().List(await db.User.ToListAsync());
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User.Public>> GetUser(int id)
        {
            if (db.User == null) return NotFound(); 

            var user = await db.User.FindAsync(id);

            return (user != null) ? new User.Public(user) : NotFound();
        }

        // GET: api/User/5/Acronyms
        [HttpGet("{id}/Acronyms")]
        public async Task<ActionResult<IEnumerable<Acronym.Dto>>> GetAcronyms(string? id)
        {
            if (db.User == null) return NotFound(); 

            var user = await db.User.Where(i => i.Id == id).Include(i => i.Acronyms).FirstOrDefaultAsync();

            if (user == null || user.Acronyms == null) return NotFound();

            return new Acronym.Dto().List(user.Acronyms.ToList());
        }

        // PUT: api/User/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> PutUser(string? id, User.Public dto)
        {
            var user = await db.User.Where(i => i.Id == id).FirstAsync();

            if (user == null) return BadRequest();

            user.Name = dto.Name;
            user.UserName = dto.UserName;
;
            db.User.Update(user);

            try { await db.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id)) return NotFound();
                else throw;
            }

            return Ok(new User.Public(user));
        }

        // POST: api/User
        // POST is disabled; new Users can only be added through the identity api end point /register
        
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // [HttpPost]
        // public async Task<ActionResult<User.Public>> PostUser(User.Public publicUser)
        // {
        //     if (db.User == null) return Problem("Entity set 'AcronymContext.User'  is null.");
        //
        //     var user = new User() { Name = publicUser.Name, UserName = publicUser.UserName };
        //
        //     db.User.Add(user);
        //     await db.SaveChangesAsync();
        //
        //     return CreatedAtAction("GetUser", new { id = publicUser.Id }, publicUser);
        // }

        // DELETE: api/User/5
        [HttpDelete("{id}"), Authorize]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (db.User == null) return NotFound(); 

            var user = await db.User.FindAsync(id);

            if (user == null) return NotFound(); 

            db.User.Remove(user);
            await db.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(string? id)
        {
            return (db.User?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
