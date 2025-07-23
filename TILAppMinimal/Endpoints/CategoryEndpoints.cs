using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using TILApp.Data;
using TILApp.Models;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace TILAppMinimal.Endpoints;

public static class CategoryEndpoints
{
    public static void MapCategoryEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Category").WithTags(nameof(Category));

        group.MapGet("/", async (Context db) => await db.Category.ToListAsync());

        group.MapGet("/{id:int}", async Task<Results<Ok<Category>, NotFound>> (int id, Context db) =>
        {
            return await db.Category.AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == id)
                is { } model
                ? Ok(model)
                : NotFound();
        });

        group.MapPut("/{id:int}", async Task<Results<Ok, NotFound>> (int id, Category category, Context db) =>
            await db.Category
                .Where(c => c.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(c => c.Id, category.Id)
                    .SetProperty(c => c.Name, category.Name)
                ) == 1 ? Ok() : NotFound());

        group.MapPost("/", async (Category category, Context db) =>
        {
            db.Category.Add(category);
            await db.SaveChangesAsync();
            return Created($"/api/Category/{category.Id}", category);
        });

        group.MapDelete("/{id:int}", async Task<Results<NoContent, NotFound>> (int id, Context db) =>
            await db.Category
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync() == 1
                ? NoContent()
                : NotFound());
    }
}