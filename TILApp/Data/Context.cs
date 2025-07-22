using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace TILApp.Data;

public class Context : IdentityDbContext<User>
{
    public Context(DbContextOptions<Context> options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
       base.OnModelCreating(modelBuilder);
       
        modelBuilder.Entity<Acronym>()
            .HasOne(a => a.User)
            .WithMany(u => u.Acronyms)
            .HasForeignKey(a => a.UserId)
            .IsRequired()  // Explicitly mark as required
            .OnDelete(DeleteBehavior.Restrict);  // Prevent parent deletion
    }
    public DbSet<Acronym> Acronym { get; set; }
    public DbSet<User> User { get; set; }
    public DbSet<Category> Category { get; set; }
    
}