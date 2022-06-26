using System.Text.Json;
using api.Data;
using api.Entities;
using api.Extensions;
using api.Helpers;
using api.Middleware;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddCors();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAppServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();

var serviceProvider = app.Services.CreateScope().ServiceProvider;

try
{
    var context = serviceProvider.GetRequiredService<DataContext>();
    var userManager = serviceProvider.GetRequiredService<UserManager<Users>>();
    var roleManager = serviceProvider.GetRequiredService<RoleManager<AppRole>>();
    await context.Database.MigrateAsync();
    Seed.SeedData(context, roleManager, userManager);
    
    var games = await context.Games.ToListAsync();
    var serializedData = JsonSerializer.Serialize(games);
    await File.WriteAllTextAsync(Directory.GetCurrentDirectory() + @"\games.txt", serializedData);
}
catch (Exception ex)
{
    var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occured during migration");
}

app.UseCors(options =>
{
    options
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
        .WithOrigins("http://localhost:4200");
});

app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthentication();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();