using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TILApp.Data;

namespace TILApp.test.TestHelpers;

// ReSharper disable once ClassNeverInstantiated.Global
public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureServices(services =>
        {
            string databaseName = $"testdb{Guid.NewGuid().ToString("N")[..10]}";
            services.AddDbContext<Context>(options => 
                options.UseNpgsql($"Host=localhost;Database={databaseName};Username=vapor_username;Password=vapor_password"));
        });
    }
}
