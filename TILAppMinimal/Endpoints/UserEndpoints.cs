using System.Runtime.InteropServices.JavaScript;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TILApp.Data;
using TILApp.Models;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace TILAppMinimal.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("minimalapi/User").WithTags(nameof(User));

        // GET: minimalapi/User
        group.MapGet("/", async (Context db) =>
            Ok(new User.Public().List(await db.User.AsNoTracking().ToListAsync())));

        // GET: minimalapi/User/5
        group.MapGet("{id}",
            async Task<Results<Ok<User.Public>, NotFound>> (string? id, Context db) =>
                await db.User.FindAsync(id)
                    is { } user
                    ? Ok(new User.Public(user))
                    : NotFound()
        );

        // GET: minimalapi/User/5/Acronyms
        group.MapGet("{id}/Acronyms", async Task<Results<Ok<IEnumerable<AcronymDto>>, NotFound>>
                (string? id, Context db) =>
                await db.User.AsNoTracking()
                        .Include(i => i.Acronyms)
                        .FirstOrDefaultAsync(u => u.Id == id)
                    is { } user
                    ? Ok(user.Acronyms.Select(a => a.ToDto()))
                    : NotFound());
        
        // POST: api/User
        // POST is disabled; new Users can only be added through the identity api end point /register

        // PUT: minimalapi/User/5
        group.MapPut("{id}",
            async Task<Results<Ok, NotFound>> (string? id, User.Public publicUser, Context db) => 
                await db.User
                .Where(u => u.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(u => u.Name, publicUser.Name)
                    .SetProperty(u => u.UserName, publicUser.UserName)
                ) == 1
                ? Ok()
                : NotFound());

        // DELETE: minimalapi/User/5


        object i = new
        {
            error = "Cannot delete user",
            message = "User has associated acronyms. Delete or reassign them first.",
            details = "This user cannot be deleted because they have acronyms associated with their account."
        };
        
        group.MapDelete("{id}",
            async Task<Results<NoContent, NotFound, Conflict<object>>> (string? id, Context db) =>
            {
                var user = await db.User
                    .Where(u => u.Id == id)
                    .Include(u => u.Acronyms)
                    .FirstOrDefaultAsync();

                if (user is null) return NotFound();

                if (user.Acronyms.Count != 0) return Conflict(i);

                db.User.Remove(user);
                await db.SaveChangesAsync();
                
                return NoContent();
            });
    }
}