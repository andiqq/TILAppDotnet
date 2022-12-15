using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUser()
        {
            if (db.User == null) return NotFound(); 

            return UserDto.convertedFrom(await db.User.ToListAsync());
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            if (db.User == null) return NotFound(); 

            var user = await db.User.FindAsync(id);

            if (user == null) return NotFound(); 

            return UserDto.convertedFrom(user);
        }

        // GET: api/User/5/Acronyms
        [HttpGet("{id}/Acronyms")]
        public async Task<ActionResult<IEnumerable<AcronymDto>>> GetAcronyms(int id)
        {
            if (db.User == null) return NotFound(); 

            var user = await db.User.Where(i => i.Id == id).Include(i => i.Acronyms).FirstOrDefaultAsync();

            if (user == null || user.Acronyms == null) return NotFound();

            return AcronymDto.convertedFrom(user.Acronyms.ToList());
        }

        // PUT: api/User/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, UserDto dto)
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

            return Ok(UserDto.convertedFrom(user));
        }

        // POST: api/User
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserDto>> PostUser(UserDto dto)
        {
            if (db.User == null) return Problem("Entity set 'AcronymContext.User'  is null.");

            var user = new User() { Name = dto.Name, UserName = dto.UserName };

            db.User.Add(user);
            await db.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = dto.Id }, dto);
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (db.User == null) return NotFound(); 

            var user = await db.User.FindAsync(id);

            if (user == null) return NotFound(); 

            db.User.Remove(user);
            await db.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return (db.User?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
