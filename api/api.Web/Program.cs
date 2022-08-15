using api.Core.Entities;
using api.Extensions;
using api.Helpers;
using api.Infrastructure.Data;
using api.Middleware;
using api.SignalR;
using API.SignalR;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class Program
{
    public async static Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddCors();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());
        builder.Services.AddAppServices(builder.Configuration);
        builder.Services.AddIdentityServices(builder.Configuration);
        builder.Services.AddSignalR();
        
        var app = builder.Build();
        
        var serviceProvider = app.Services.CreateScope().ServiceProvider;
        
        try
        {
            var context = serviceProvider.GetRequiredService<DataContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<Users>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<AppRole>>();
            await context.Database.MigrateAsync();
            Seed.SeedData(context, roleManager, userManager);
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
        
        app.MapControllers();
        
        app.UseRouting();
        
        app.UseAuthorization();
        
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHub<PresenceHub>("hubs/presence");
            endpoints.MapHub<MessageHub>("hubs/message");
        });
        
        app.Run();
    }
}