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

        // GET: minimalapi/Acronym
        group.MapGet("/", async (Context db) =>
            Ok(await db.Acronym.Select(a => a.ToDto()).ToListAsync()))
            .WithName("GetAllAcronyms")
            .WithOpenApi();

        // GET: minimalapi/Acronym/5
        group.MapGet("{id:int}",
                async Task<Results<Ok<AcronymDto>, NotFound>> (int id, Context db) =>
                    await db.Acronym.FindAsync(id)
                        is Acronym acronym
                        ? Ok(acronym.ToDto())
                        : NotFound())
            .WithName("GetAcronymById")
            .WithOpenApi();

        //GET: minimalapi/Acronym/5/categories
        group.MapGet("{id:int}/Categories",
                async Task<Results<Ok<List<CategoryDto>>, NotFound>> (int id, Context db) =>
                    await db.Acronym.AsNoTracking()
                            .Include(a => a.Categories)
                            .FirstOrDefaultAsync(a => a.Id == id)
                        is { Categories.Count: not 0} acronym
                        ? Ok(acronym.Categories.Select(c => c.ToDto()).ToList())
                        : NotFound())
            .WithName("GetAcronymCategories")
            .WithOpenApi();

        //GET: minimalapi/Acronym/5/User
        group.MapGet("{id:int}/User",
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
                async Task<Results<Ok<List<AcronymDto>>, NotFound>> (string term, Context db) =>
                {
                    var acronyms = await db.Acronym.AsNoTracking()
                        .Where(a => a.Short.Contains(term) ||
                                    a.Long.Contains(term)
                        ).ToListAsync();

                    return acronyms.Count == 0
                        ? NotFound()
                        : Ok(acronyms.Select(a => a.ToDto()).ToList());
                })
            .WithName("SearchForAcronymWithTerm")
            .WithOpenApi();

        // GET: minimalapi/Acronym/first
        group.MapGet("first",
                async Task<Results<Ok<AcronymDto>, NotFound>> (Context db) =>
                    await db.Acronym.AsNoTracking().FirstAsync()
                        is { } acronym
                        ? Ok(acronym.ToDto())
                        : NotFound())
            .WithName("GetFirstAcronym")
            .WithOpenApi();

        // GET: minimalapi/Acronym/sort
        group.MapGet("sort",
                async Task<Results<Ok<List<AcronymDto>>, NotFound>> (Context db) =>
                     Ok(await db.Acronym.AsNoTracking()
                                .OrderBy(a => a.Short).Select(a => a.ToDto())
                                .ToListAsync()))
            .WithName("SortAcronyms")
            .WithOpenApi();

        // PUT: minimalapi/Acronym/5
        group.MapPut("{id:int}",
                async Task<Results<Ok, BadRequest, NotFound>> (int id, AcronymDto acronym, Context db) =>
                {
                    if (await db.Acronym.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id) is null)
                        return NotFound();
                    if (await db.User.AsNoTracking().FirstOrDefaultAsync(u => u.Id == acronym.UserId) is null)
                        return BadRequest();

                    return await db.Acronym
                        .Where(a => a.Id == id)
                        .ExecuteUpdateAsync(setters => setters
                            .SetProperty(m => m.Id, acronym.Id)
                            .SetProperty(m => m.Long, acronym.Long)
                            .SetProperty(m => m.Short, acronym.Short)
                            .SetProperty(m => m.UserId, acronym.UserId)
                        ) == 1
                        ? Ok()
                        : BadRequest();
                    //        return affected == 1 ? Ok() : BadRequest();
                })
          //  .RequireAuthorization()
            // TODO not active; .NET Identity is not yet part of the project
            .WithName("UpdateAcronym")
            .WithOpenApi();
    }
}