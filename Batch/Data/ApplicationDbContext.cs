using Batch.Data;
using Microsoft.EntityFrameworkCore;

namespace Batch.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)  // ← ADD THIS LINE!
    {
        
    }
    
    // DbSet
    public DbSet<User> Users { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>().ToTable("users", "dbo");
    }
}