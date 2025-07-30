using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
            Ok(await db.Acronym.Select(a => a.ToDto()).ToListAsync()));

        // GET: minimalapi/Acronym/5
        group.MapGet("{id:int}",
                async Task<Results<Ok<AcronymDto>, NotFound>> (int id, Context db) =>
                    await db.Acronym.FindAsync(id) is Acronym acronym
                        ? Ok(acronym.ToDto())
                        : NotFound());

        //GET: minimalapi/Acronym/5/categories
        group.MapGet("{id:int}/Categories",
                async Task<Results<Ok<IEnumerable<CategoryDto>>, NotFound>> (int id, Context db) =>
                    await db.Acronym.AsNoTracking()
                            .Include(a => a.Categories)
                            .FirstOrDefaultAsync(a => a.Id == id)
                        is { } acronym
                        ? Ok(acronym.Categories.Select(c => c.ToDto()))
                        : NotFound());

        //GET: minimalapi/Acronym/5/User
        group.MapGet("{id:int}/User",
                async Task<Results<Ok<User.Public>, NotFound>> (int id, Context db) =>
                    await db.Acronym.AsNoTracking()
                            .Include(a => a.User)
                            .FirstOrDefaultAsync(a => a.Id == id)
                        is { } acronym
                        ? Ok(acronym.User is null 
                            ? null 
                            : new User.Public(acronym.User))
                        : NotFound());

        // GET: minimalapi/Acronym/search?Term=TIL
        group.MapGet("search",
            async Task (string term, Context db) 
                => Ok(await db.Acronym.AsNoTracking()
                        .Where(a => a.Short == term || a.Long == term)
                        .Select(a => a.ToDto())
                        .ToListAsync()));

        // GET: minimalapi/Acronym/first
        group.MapGet("first",
                async Task<Results<Ok<AcronymDto>, NotFound>> (Context db) =>
                    await db.Acronym.AsNoTracking().FirstAsync()
                        is { } acronym
                        ? Ok(acronym.ToDto())
                        : NotFound());

        // GET: minimalapi/Acronym/sort
        group.MapGet("sort",
                async Task<Results<Ok<List<AcronymDto>>, NotFound>> (Context db) =>
                     Ok(await db.Acronym.AsNoTracking()
                                .OrderBy(a => a.Short)
                                .Select(a => a.ToDto())
                                .ToListAsync()));
        
        // POST: minimalap/Acronym
        group.MapPost("/",
            async Task<Results<Ok<AcronymDto>, BadRequest>> (AcronymDto dto, Context db) =>
            {
                if (await db.User.FirstOrDefaultAsync(u => u.Id == dto.UserId) == null)
                    return BadRequest();
                
                var acronym = new Acronym
                {
                    Short = dto.Short,
                    Long = dto.Long,
                    UserId = dto.UserId
                };

                await db.Acronym.AddAsync(acronym);
                await db.SaveChangesAsync();

                return Ok(acronym.ToDto());
            });

        // PUT: minimalapi/Acronym/5
        group.MapPut("{id:int}",
                async Task<Results<Ok, BadRequest, NotFound>> (int id, AcronymDto dto, Context db) =>
                {
                    // Foreign key constraint
                    if (await db.User.FindAsync(dto.UserId) == null)
                        return BadRequest();
                    
                    return await db.Acronym
                        .Where(a => a.Id == id)
                        .ExecuteUpdateAsync(setters => setters
                            .SetProperty(a => a.Long, dto.Long)
                            .SetProperty(a => a.Short, dto.Short)
                            .SetProperty(a => a.UserId, dto.UserId)
                        ) == 1
                        ? Ok() : NotFound();
                });
        
        // DELETE: minimalapi/Acronym/5
        group.MapDelete("{id:int}", async Task<Results<NoContent, NotFound>> (int id, Context db) =>
            await db.Acronym
                .Where(a => a.Id == id)
                .ExecuteDeleteAsync() == 1
                ? NoContent()
                : NotFound());
    }
}