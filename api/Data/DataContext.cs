using api.Entities;
using Microsoft.EntityFrameworkCore;

namespace api.Data;

public class DataContext : DbContext
{
    public DataContext() {}
    
    public DataContext(DbContextOptions options) : base(options) {}

    public DbSet<Users> User { get; set; }

    public DbSet<Games> Games { get; set; }

    public DbSet<UserGames> UserGames { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserGames>().HasKey(key => new {key.GameId, key.UserId});

        modelBuilder.Entity<UserGames>()
            .HasOne(ug => ug.User)
            .WithMany(u => u.Colection)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserGames>()
            .HasOne(ug => ug.Game)
            .WithMany(g => g.collection)
            .HasForeignKey(f => f.GameId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}