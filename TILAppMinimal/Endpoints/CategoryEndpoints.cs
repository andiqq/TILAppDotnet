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

        group.MapGet("/", async (Context db) =>
            Ok(await db.Category.AsNoTracking()
                .Select(c => c.ToDto())
                .ToListAsync()));

        group.MapGet("/{id:int}",
            async Task<Results<Ok<CategoryDto>, NotFound>> (int id, Context db) =>
                await db.Category.FindAsync(id)
                    is { } category
                    ? Ok(category.ToDto())
                    : NotFound());
        
        group.MapPost("/", async (CategoryDto dto, Context db) =>
        {
            var category = new Category() { Name = dto.Name };
            
            db.Category.Add(category);
            await db.SaveChangesAsync();
            
            return Created($"/api/Category/{category.Id}", category);
        });

        group.MapPut("/{id:int}", 
            async Task<Results<Ok<CategoryDto>, NotFound>> (int id, CategoryDto dto, Context db) =>
            await db.Category
                .Where(c => c.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(c => c.Id, dto.Id)
                    .SetProperty(c => c.Name, dto.Name)
                ) == 1
                ? Ok(dto)
                : NotFound());

        

        group.MapDelete("/{id:int}", async Task<Results<NoContent, NotFound>> (int id, Context db) =>
            await db.Category
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync() == 1
                ? NoContent()
                : NotFound());
    }
}