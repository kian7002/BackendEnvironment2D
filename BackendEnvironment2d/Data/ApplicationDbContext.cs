using Backend.Models;
using BackendEnvironment2d.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Environment2D> Environments => Set<Environment2D>();
    public DbSet<Object2D> Objects => Set<Object2D>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Environment2D>()
            .HasIndex(e => new { e.UserId, e.Name })
            .IsUnique();

        builder.Entity<Environment2D>()
            .HasMany(e => e.Objects)
            .WithOne(o => o.Environment!)
            .HasForeignKey(o => o.EnvironmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}