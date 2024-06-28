using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

public class Context : IdentityDbContext<User>
{
    public Context(DbContextOptions<Context> options) : base(options) { }

    public DbSet<Acronym> Acronym { get; set; }

    public DbSet<User> User { get; set; }

    public DbSet<Category> Category { get; set; }

}
