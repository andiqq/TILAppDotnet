using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TILApp.Data;
using TILApp.Models;
using TILAppMinimal.test.TestHelpers;
using Xunit;

namespace TILAppMinimal.test;

public class CategoryEndpointsTest : IClassFixture<CustomWebApplicationFactory<Program>>, IDisposable
{
    private readonly HttpClient client;
    private readonly Context db;

    public CategoryEndpointsTest(CustomWebApplicationFactory<Program> factory)
    {
        client = factory.CreateClient();
        db = factory.Services.CreateScope().ServiceProvider.GetRequiredService<Context>();
        db.Database.EnsureDeleted();
        db.Database.Migrate();
        db.Category.Add(new Category { Name = "Classics" });
        db.SaveChanges();
    }
    
    [Fact]
    public async Task GetCategory()
    {
        var response = await client.GetAsync("/minimalapi/Category/1");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await response.Content.ReadFromJsonAsync<CategoryDto>()).Name.Should().Be("Classics");
        
        response = await client.GetAsync("/minimalapi/Category/2");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    [Fact]
    public async Task GetCategories()
    {
        var response = await client.GetAsync("/minimalapi/Category");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var categories = (await response.Content.ReadFromJsonAsync<IEnumerable<CategoryDto>>()).ToList();
        categories.Count.Should().Be(1);
    }

    [Fact]
    public async Task PostCategorie()
    {
        var category = new CategoryDto() { Name = "Modern" }; 
        var response = await client.PostAsJsonAsync("/minimalapi/Category", category);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        category = await response.Content.ReadFromJsonAsync<CategoryDto>();
        category.Id.Should().Be(2);
        category.Name.Should().Be("Modern");
    }

    [Fact]
    public async Task PutCategorie()
    {
        var category = new CategoryDto() { Name = "Modern" }; 
        await client.PostAsJsonAsync("/minimalapi/Category", category);
        category = new CategoryDto() { Name = "PostModern" };
        var response = await client.PutAsJsonAsync("/minimalapi/Category/2", category);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        category = await response.Content.ReadFromJsonAsync<CategoryDto>();
        category.Id.Should().Be(2);
        category.Name.Should().Be("PostModern");
    }

    [Fact]
    public async Task DeleteCategorie()
    {
        var response = await client.DeleteAsync("/minimalapi/Category/1");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        response = await client.GetAsync("minimalapi/Category/1");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    public void Dispose()
    {
        client.Dispose();
        db.Database.EnsureDeleted();
        db.Dispose();
    }
}
