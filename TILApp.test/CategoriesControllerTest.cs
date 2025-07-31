using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TILApp.Data;
using TILApp.Models;

namespace TILApp.test;

public class CategoriesControllerTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient client;

    public CategoriesControllerTest(WebApplicationFactory<Program> factory)
    {
        client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddDbContext<Context>(options => 
                    options.UseNpgsql("Host=localhost;Database=testdb;Username=vapor_username;Password=vapor_password"));
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<Context>();
                db.Database.EnsureDeleted();
                db.Database.Migrate();
                db.Category.Add(new Category() { Name = "Classics" });
                db.SaveChangesAsync();
            });
        }).CreateClient();
    }
    
    [Fact]
    public async Task GetCategory()
    {
        var response = await client.GetAsync("/api/Category/1");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await response.Content.ReadFromJsonAsync<CategoryDto>()).Name.Should().Be("Classics");
    }
    [Fact]
    public async Task GetCategories()
    {
        var response = await client.GetAsync("/api/Category");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await response.Content.ReadFromJsonAsync<CategoryDto>()).Name.Should().Be("Classics");
    }
    
    
   
    
    
}