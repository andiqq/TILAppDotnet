using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TILApp.Models;

namespace TILAppMinimal.Endpoints;

public static class AcronymEndpoints
{
    public static void MapAcronymEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("minimalapi/Acronym").WithTags(nameof(Acronym));

        group.MapGet("/", async (Context db) =>
        {
            return new Acronym.Dto().List(await db.Acronym.ToListAsync());
        })
        .WithName("GetAllAcronyms")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<Acronym>, NotFound>> (int id, Context db) =>
        {
            return await db.Acronym.AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id) 
                is Acronym acronym
                ? TypedResults.Ok(acronym)
                : TypedResults.NotFound();
        })
        .WithName("GetAcronymById");
    }
}