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
        group.MapGet("{id}/Acronyms", async Task<Results<Ok<IEnumerable<ShortAcronym>>, NotFound>>
                (string? id, Context db) =>
                await db.User.AsNoTracking()
                        .Include(i => i.Acronyms)
                        .FirstOrDefaultAsync(u => u.Id == id)
                    is { } user
                    ? Ok(user.Acronyms.Select(a => a.ToShort()))
                    : NotFound());

        // PUT: minimalapi/User/5
        group.MapPut("{id}",
            async Task<Results<Ok<User.Public>, NotFound>> (string? id, User.Public publicUser, Context db) =>
            {
                var user = await db.User.FindAsync(id);

                if (user == null) return NotFound();

                user.Name ??= publicUser.Name;
                user.UserName ??= publicUser.UserName;

                db.User.Update(user);
                await db.SaveChangesAsync();

                return Ok(new User.Public(user));
            }
        );

        // DELETE: minimalapi/User/5
        group.MapDelete("{id}",
            async Task<Results<NoContent, NotFound>> (string? id, Context db) =>
                await db.User
                    .Where(u => u.Id == id)
                    .ExecuteDeleteAsync() == 1
                    ? NoContent()
                    : NotFound());
    }
}