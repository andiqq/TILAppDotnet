using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TILApp.Data;
using TILApp.Models;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace TILAppMinimal.Endpoints;

public static class AcronymEndpoints
{
    public static void MapAcronymEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("minimalapi/Acronym").WithTags(nameof(Acronym));

        // GET: minimalapi/Acronym
        group.MapGet("/", async (Context db) =>
            Ok(await db.Acronym
                .AsNoTracking()
                .Select(a => a.ToDto())
                .ToListAsync()));

        // GET: minimalapi/Acronym/5
        group.MapGet("{id:int}",
                async Task<Results<Ok<AcronymDto>, NotFound>> (int id, Context db) =>
                    await db.Acronym.FindAsync(id) is { } acronym
                        ? Ok(acronym.ToDto())
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
                await db.Acronym.AsNoTracking().FirstAsync() is { } acronym
                    ? Ok(acronym.ToDto())
                    : NotFound());

        // GET: minimalapi/Acronym/sort
        group.MapGet("sort",
            async Task (Context db) =>
                Ok(await db.Acronym.AsNoTracking()
                    .OrderBy(a => a.Short)
                    .Select(a => a.ToDto())
                    .ToListAsync()));
        
        //GET: minimalapi/Acronym/5/User
        group.MapGet("{id:int}/User",
            async Task<Results<Ok<User.Public>, NotFound>> (int id, Context db) =>
                await db.Acronym.AsNoTracking()
                        .Include(a => a.User)
                        .FirstOrDefaultAsync(a => a.Id == id)
                    is { } acronym
                    ? Ok(new User.Public(acronym.User))
                    : NotFound());

        //GET: minimalapi/Acronym/5/Categories
        group.MapGet("{id:int}/Categories",
                async Task<Results<Ok<IEnumerable<CategoryDto>>, NotFound>> (int id, Context db) =>
                    await db.Acronym.AsNoTracking()
                            .Include(a => a.Categories)
                            .FirstOrDefaultAsync(a => a.Id == id)
                        is { } acronym
                        ? Ok(acronym.Categories.Select(c => c.ToDto()))
                        : NotFound());
        
        // POST: minimalap/Acronym
        group.MapPost("/",
            async Task<Results<Created<AcronymDto>, BadRequest>> (AcronymDto dto, Context db) =>
            {
                if (await db.User.FindAsync(dto.UserId) == null)
                    return BadRequest();
                
                var acronym = new Acronym
                {
                    Short = dto.Short,
                    Long = dto.Long,
                    UserId = dto.UserId
                };

                await db.Acronym.AddAsync(acronym);
                await db.SaveChangesAsync();

                return Created($"{acronym.Id}", acronym.ToDto());
            });

        // PUT: minimalapi/Acronym/5
        group.MapPut("{id:int}",
                async Task<Results<Ok<AcronymDto>, BadRequest, NotFound>> (int id, AcronymDto dto, Context db) =>
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
                        ? Ok(dto) : NotFound();
                });
        
        // PUT: minimalapi/Acroym/5/Category
        group.MapPut("{id:int}/Category",
            async Task<Results<Created<CategoryDto>, NotFound>> (int id, int catid, Context db) =>
            {
                var acronym = await db.Acronym
                    .Include(a => a.Categories)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (acronym == null) return NotFound();

                var category = await db.Category.FindAsync(catid);

                if (category == null) return NotFound();

                acronym.Categories.Add(category);
                db.Acronym.Update(acronym);
                await db.SaveChangesAsync();
                
                return Created($"{id:int}/Categories", category.ToDto());
            });
            
        
        // DELETE: minimalapi/Acronym/5
        group.MapDelete("{id:int}", 
            async Task<Results<NoContent, NotFound>> (int id, Context db) =>
            await db.Acronym
                .Where(a => a.Id == id)
                .ExecuteDeleteAsync() == 1
                ? NoContent()
                : NotFound());

        group.MapDelete("{id:int}/Category",
            async Task<Results<NoContent, NotFound>> (int id, int catid, Context db) =>
        {
            var acronym = await db.Acronym
                .Where(i => i.Id == id)
                .Include(a => a.Categories)
                .FirstOrDefaultAsync();
            
            if (acronym == null) return NotFound();

            var category = acronym.Categories.FirstOrDefault(c => c.Id == catid);

            if (category == null) return NotFound();
            
            acronym.Categories.Remove(category);

            db.Acronym.Update(acronym);
            await db.SaveChangesAsync();

            return NoContent();
        });
    }
}