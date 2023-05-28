using System.Text.Json;
using api.Core.Entities;
using api.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace api.Helpers;

public class Seed
{
    public static async void SeedData(DataContext context, RoleManager<AppRole> roleManager,
        UserManager<Users> userManager)
    {
        if (!await context.Games.AnyAsync() && File.Exists(@"games.txt"))
        {
            var gamesJson = await File.ReadAllTextAsync(Directory.GetCurrentDirectory() + @"\games.txt");
            var games = JsonSerializer.Deserialize<List<Games>>(gamesJson);
            await context.Games.AddRangeAsync(games);
            await context.SaveChangesAsync();
        }

        if (!await roleManager.Roles.AnyAsync())
        {
            var roles = new List<AppRole>
            {
                new() {Name = "member"},
                new() {Name = "admin"}
            };

            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);
            }
        }

        if (!await userManager.Users.AnyAsync(u => u.NormalizedUserName == "ADMIN"))
        {
            var admin = new Users();
            admin.UserName = "admin";
            await userManager.CreateAsync(admin, "admin1");
            await userManager.AddToRolesAsync(admin, new[] {"member", "admin"});
        }
    }
}