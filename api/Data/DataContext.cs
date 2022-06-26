using api.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace api.Data;

public class DataContext : IdentityDbContext<Users, AppRole, int, IdentityUserClaim<int>,
    AppUserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
{
    public DataContext() {}
    
    public DataContext(DbContextOptions options) : base(options) {}

    public DbSet<Games> Games { get; set; }

    public DbSet<UserGames> UserGames { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserGames>().HasKey(key => new {key.GameId, key.UserId});

        modelBuilder.Entity<UserGames>()
            .HasOne(ug => ug.User)
            .WithMany(u => u.Collection)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Games>().HasAlternateKey(key => key.AppId);

        modelBuilder.Entity<UserGames>()
            .HasOne(ug => ug.Game)
            .WithMany(g => g.collection)
            .HasForeignKey(f => f.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Users>()
            .HasMany(ur => ur.UserRoles)
            .WithOne(u => u.User)
            .HasForeignKey(ur => ur.UserId)
            .IsRequired();

        modelBuilder.Entity<AppRole>()
            .HasMany(ar => ar.UserRoles)
            .WithOne(r => r.Role)
            .HasForeignKey(f => f.RoleId)
            .IsRequired();
    }
}