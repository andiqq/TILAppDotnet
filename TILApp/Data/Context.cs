using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace TILApp.Data;

public class Context(DbContextOptions<Context> options) : IdentityDbContext<User>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Acronym>()
            .HasOne(a => a.User)
            .WithMany(u => u.Acronyms)
            .HasForeignKey(c => c.UserId)
            .IsRequired()  // Explicitly mark as required
            .OnDelete(DeleteBehavior.Restrict);  // Prevent parent deletion
    }
    public DbSet<Acronym> Acronym { get; set; }
    public DbSet<User> User { get; set; }
    public DbSet<Category> Category { get; set; }
    
}