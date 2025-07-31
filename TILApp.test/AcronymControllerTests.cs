using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.VisualStudio.Web.CodeGeneration.Design;
using TILApp.Data;
using TILApp.Models;

namespace TILApp.test;

public class AcronymControllerTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly HttpClient client;
    private readonly Context db;

    public AcronymControllerTests(WebApplicationFactory<Program> factory)
    {
        client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<DbContextOptions<Context>>();
                services.AddDbContext<Context>(opt =>
                    opt.UseInMemoryDatabase("TestDB"));
            });
        }).CreateClient();

        db = factory.Services.GetRequiredService<Context>();
        db.Category.Add(new Category() { Name = "Classics" });
        db.SaveChanges();
    }

    [Fact]
    public async Task GetCategories()
    {
        var response = await client.GetAsync("/api/Category/1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await response.Content.ReadFromJsonAsync<CategoryDto>()).Name.Should().Be("Classics");
    }
    
    public void Dispose()
    {
        db.Database.EnsureDeleted();
    }
}