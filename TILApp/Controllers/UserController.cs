using TILApp.Data;

namespace TILApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(Context db) : ControllerBase
    {
        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User.Public>>> GetUser() =>
            Ok(new User.Public().List(await db.User.AsNoTracking().ToListAsync()));
        
        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User.Public>> GetUser(int id) =>
            await db.User.FindAsync(id) is { } user
                ? Ok(new User.Public(user))
                : NotFound();
        
        // GET: api/User/5/Acronyms
        [HttpGet("{id}/Acronyms")]
        public async Task<ActionResult<IEnumerable<AcronymDto>>> GetAcronyms(string? id) =>
            await db.User.AsNoTracking()
                    .Include(i => i.Acronyms)
                    .FirstOrDefaultAsync(u => u.Id == id)
                is { } user
                ? Ok(user.Acronyms.Select(a => a.ToDto()))
                : NotFound();

        // PUT: api/User/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> PutUser(string? id, User.Public dto) =>
            await db.User
                .Where(u => u.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(u => u.Name, dto.Name)
                    .SetProperty(u => u.UserName, dto.UserName)
                ) == 1
                ? Ok()
                : NotFound();

        // POST: api/User
        // POST is disabled; new Users can only be added through the identity api end point /register

        // DELETE: api/User/5
        
        [HttpDelete("{id}"), Authorize]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await db.User
                .Where(u => u.Id == id)
                .Include(u => u.Acronyms)
                .FirstOrDefaultAsync();

            if (user is null) return NotFound();
            
            if (user.Acronyms.Count != 0) return Conflict(new
            {
                error = "Cannot delete user",
                message = "User has associated acronyms. Delete or reassign them first.",
                details = "This user cannot be deleted because they have acronyms associated with their account."
            });

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