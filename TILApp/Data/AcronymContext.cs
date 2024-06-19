using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

public class AcronymContext : IdentityDbContext<User>
{
    public AcronymContext(DbContextOptions<AcronymContext> options) : base(options) { }

    public DbSet<Acronym> Acronym { get; set; }

    public DbSet<User> User { get; set; }

    public DbSet<Category> Category { get; set; }

}
