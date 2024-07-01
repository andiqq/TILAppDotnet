using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TILApp.Data;
using TILApp.Models;
using static Microsoft.AspNetCore.Http.TypedResults;

// ReSharper disable ConvertTypeCheckPatternToNullCheck

namespace TILAppMinimal.Endpoints;

public static class AcronymEndpoints
{
    public static void MapAcronymEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("minimalapi/Acronym").WithTags(nameof(Acronym));

        // GET: minmalapi/Acronym
        group.MapGet("/", async (Context db) => new Acronym.Dto().List(await db.Acronym.ToListAsync()))
            .WithName("GetAllAcronyms")
            .WithOpenApi();

        // GET: minimalapi/Acronym/5
        group.MapGet("{id}",
                async Task<Results<Ok<Acronym.Dto>, NotFound>> (int id, Context db) =>
                    await db.Acronym.FindAsync(id)
                        is Acronym acronym
                        ? Ok(new Acronym.Dto(acronym))
                        : NotFound())
            .WithName("GetAcronymById")
            .WithOpenApi();

        //GET: minimalapi/Acronym/5/categories
        group.MapGet("{id}/Categories",
                async Task<Results<Ok<List<CategoryDto>>, NotFound>> (int id, Context db) =>
                    await db.Acronym.AsNoTracking()
                            .Include(a => a.Categories)
                            .FirstOrDefaultAsync(a => a.Id == id)
                        is { Categories.Count: not 0} acronym
                        ? Ok(acronym.Categories.Select(c => c.toDto()).ToList())
                        : NotFound())
            .WithName("GetAcronymCategories")
            .WithOpenApi();

        //GET: minimalapi/Acronym/5/User
        group.MapGet("{id}/User",
                async Task<Results<Ok<User.Public>, NotFound>> (int id, Context db) =>
                    await db.Acronym.AsNoTracking()
                            .Include(a => a.User)
                            .FirstOrDefaultAsync(a => a.Id == id)
                        is { User: not null } acronym
                        ? Ok(new User.Public(acronym.User))
                        : NotFound())
            .WithName("GetAcronymUser")
            .WithOpenApi();

        // GET: minimalapi/Acronym/search?Term=OMG
        group.MapGet("search",
                async Task<Results<Ok<List<Acronym.Dto>>, NotFound>> (string term, Context db) =>
                {
                    var acronyms = await db.Acronym.AsNoTracking()
                        .Where(a => a.Short.Contains(term) ||
                                    a.Long.Contains(term)
                        ).ToListAsync();

                    return acronyms is null || acronyms.Count == 0
                        ? NotFound()
                        : Ok(new Acronym.Dto().List(acronyms));
                })
            .WithName("SearchForAcronymWithTerm")
            .WithOpenApi();

        // GET: minimalapi/Acronym/first
        group.MapGet("first",
                async Task<Results<Ok<Acronym.Dto>, NotFound>> (Context db) =>
                    await db.Acronym.AsNoTracking().FirstAsync()
                        is { } acronym
                        ? Ok(new Acronym.Dto(acronym))
                        : NotFound())
            .WithName("GetFirstAcronym")
            .WithOpenApi();

        // GET: minimalapi/Acronym/sort
        group.MapGet("sort",
                async Task<Results<Ok<List<Acronym.Dto>>, NotFound>> (Context db) =>
                    db.Acronym is null
                        ? NotFound()
                        : Ok(new Acronym.Dto().List(
                            await db.Acronym.AsNoTracking()
                                .OrderBy(a => a.Short)
                                .ToListAsync())
                        ))
            .WithName("SortAcronyms")
            .WithOpenApi();

        // PUT: minimalapi/Acronym/5
        group.MapPut("{id}",
                async Task<Results<Ok, BadRequest, NotFound>> (int id, Acronym.Dto acronym, Context db) =>
                {
                    if (await db.Acronym.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id) is null)
                        return NotFound();
                    if (await db.User.AsNoTracking().FirstOrDefaultAsync(u => u.Id == acronym.UserId) is null)
                        return BadRequest();

                    var affected = await db.Acronym
                        .Where(a => a.Id == id)
                        .ExecuteUpdateAsync(setters => setters
                            .SetProperty(m => m.Id, acronym.Id)
                            .SetProperty(m => m.Long, acronym.Long)
                            .SetProperty(m => m.Short, acronym.Short)
                            .SetProperty(m => m.UserId, acronym.UserId)
                        );
                    return affected == 1 ? Ok() : BadRequest();
                })
          //  .RequireAuthorization()
            // TODO not active; .NET Identity is not yet part of the project
            .WithName("UpdateAcronym")
            .WithOpenApi();
    }
}