using System.Net;
using System.Text;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TILApp.Data;
using TILApp.Models;
using TILApp.test.TestHelpers;
using Xunit;
using Xunit.Abstractions;

namespace TILApp.test;

public class AcronymEndpointsTest : IClassFixture<CustomWebApplicationFactory<Program>>, IDisposable
{
    private readonly ITestOutputHelper testOutputHelper;
    private readonly HttpClient client;
    private readonly Context db;

    public AcronymEndpointsTest(CustomWebApplicationFactory<Program> factory, ITestOutputHelper testOutputHelper)
    {
        this.testOutputHelper = testOutputHelper;
        client = factory.CreateClient();
        db = factory.Services.CreateScope().ServiceProvider.GetRequiredService<Context>();
        db.Database.EnsureDeleted();
        db.Database.Migrate();
        var user = new User
        {
            Id = "fa4c76f8-9746-4f14-8104-b8e441788475",
            UserName = "eolsen@web.de",
            NormalizedUserName = "EOLSEN@WEB.DE",
            Email = "eolsen@web.de",
            NormalizedEmail = "EOLSEN@WEB.DE",
            EmailConfirmed = false,
            PasswordHash = "AQAAAAIAAYagAAAAEI5Eq6bp9Hfzepqd9zI23VJLRmiQF2Jnwu2rzyh/f91pqzA+dbfsZIGRsMIKdPdg5w==",
            SecurityStamp = "QOT7WMH7GBX5YHFFWTXWNLKA3MBQO7HC",
            ConcurrencyStamp = "1d2779b5-af95-4d2a-a2d0-926087fe8570",
        };
        db.User.Add(user);
        db.SaveChanges();
    }

    [Fact]
    public async Task GetUsers()
    {
        var response = await client.GetAsync("/minimalapi/User");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var users = (await response.Content.ReadFromJsonAsync<IEnumerable<User>>()).ToList();
        users[0].Id.Should().Be("fa4c76f8-9746-4f14-8104-b8e441788475");
    }
    
    public void Dispose()
    {
        client.Dispose();
        db.Database.EnsureDeleted();
        db.Dispose();
    }
}