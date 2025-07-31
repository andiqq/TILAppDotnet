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

public class CategoriesControllerTest
{
    private readonly HttpClient client;
    public CategoriesControllerTest()
    {
        client = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddDbContext<Context>(options => 
                        options.UseNpgsql("Host=localhost;Database=testdb;Username=vapor_username;Password=vapor_password"));
                    using var db = services
                        .BuildServiceProvider()
                        .CreateScope()
                        .ServiceProvider
                        .GetRequiredService<Context>();
                    db.Database.EnsureDeleted();
                    db.Database.Migrate();
                    db.Category.Add(new Category() { Name = "Classics" });
                    db.SaveChanges();
                });
            }).CreateClient();
    }
    
    [Fact]
    public async Task GetCategory()
    {
        var response = await client.GetAsync("/minimalapi/Category/1");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await response.Content.ReadFromJsonAsync<CategoryDto>()).Name.Should().Be("Classics");
    }
    [Fact]
    public async Task GetCategories()
    {
        var response = await client.GetAsync("/minimalapi/Category");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var categories = (await response.Content.ReadFromJsonAsync<IEnumerable<CategoryDto>>()).ToArray();
        categories[0].Name.Should().Be("Classics");
        categories.Length.Should().Be(1);
    }
}