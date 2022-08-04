using api.Helpers;
using api.infrastructure.Data;
using api.infrastructure.Interfaces;
using api.Services;
using api.SignalR;
using Microsoft.EntityFrameworkCore;

namespace api.Extensions;

public static class AppServicesExtension
{
    public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<DataContext>(options =>
        {
            options.UseSqlite(config.GetConnectionString("default"));
        });

        services
            .AddSingleton<PresenceTracker>()
            .AddScoped<ITokenService, TokenService>()
            .AddScoped<IAccountRepository, AccountRepository>()
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .AddAutoMapper(typeof(AutomapperProfile).Assembly);

        return services;
    }
}