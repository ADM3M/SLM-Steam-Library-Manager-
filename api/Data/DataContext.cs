using api.Entities;
using Microsoft.EntityFrameworkCore;

namespace api.Data;

public class DataContext : DbContext
{
    public DataContext() {}
    
    public DataContext(DbContextOptions options) : base(options) {}

    public DbSet<User> User { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        
    }
}