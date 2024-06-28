using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TILApp.Data;
using TILApp.Models;

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
                {
                    var acronym = await db.Acronym.AsNoTracking()
                        .FirstOrDefaultAsync(a => a.Id == id);
                    return acronym != null
                        ? TypedResults.Ok(new Acronym.Dto(acronym))
                        : TypedResults.NotFound();
                })
            .WithName("GetAcronymById")
            .WithOpenApi();

        //GET: minimalapi/Acronym/5/categories
        group.MapGet("{id}/Categories",
                async Task<Results<Ok<List<Category.CDto>>, NotFound>> (int id, Context db) =>
                {
                    var acronym = await db.Acronym.AsNoTracking()
                        .Include(a => a.Categories)
                        .FirstOrDefaultAsync(a => a.Id == id);
                    return acronym is not null
                        ? TypedResults.Ok(new Category.CDto().List(acronym.Categories.ToList()))
                        : TypedResults.NotFound();
                })
            .WithName("GetAcronymCategories")
            .WithOpenApi();

        //GET: minimalapi/Acronym/5/User
        group.MapGet("{id}/User",
                async Task<Results<Ok<User.Public>, NotFound>> (int id, Context db) =>
                {
                    if (db.Acronym is null) return TypedResults.NotFound();
                    var acronym = await db.Acronym.FindAsync(id);

                    if (acronym is null) return TypedResults.NotFound();
                    var user = await db.User.FindAsync(acronym.UserId);

                    return user is null
                        ? TypedResults.NotFound()
                        : TypedResults.Ok(new User.Public(user));
                })
            .WithName("GetAcronymUser")
            .WithOpenApi();

        //GET: minimalapi/Acronym/search?Term=OMG
        group.MapGet("search",
            async Task<Results<Ok<List<Acronym.Dto>>, NotFound>> (string term, Context db) =>
            {
                var acronyms = await db.Acronym.AsNoTracking()
                    .Where(a => a.Short.Contains(term) ||
                                a.Long.Contains(term)
                    ).ToListAsync();

                return acronyms is null || acronyms.Count == 0
                    ? TypedResults.NotFound()
                    : TypedResults.Ok(new Acronym.Dto().List(acronyms));
            })
            .WithName("SearchForAcronymWithTerm")
            .WithOpenApi();
    }
}