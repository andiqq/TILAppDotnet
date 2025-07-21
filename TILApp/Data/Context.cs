using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace TILApp.Data;

public class Context(DbContextOptions<Context> options) : IdentityDbContext<User>(options)
{
    public DbSet<Acronym> Acronym { get; set; }
    public DbSet<User> User { get; set; }
    public DbSet<Category> Category { get; set; }
}