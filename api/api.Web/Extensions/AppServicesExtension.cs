using api.Application.Helpers;
using api.Application.Interfaces;
using api.Application.Services;
using api.Infrastructure.Data;
using api.SignalR;
using Microsoft.EntityFrameworkCore;

namespace api.Extensions;

public static class AppServicesExtension
{
    public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<DataContext>(options =>
        {
            options.UseSqlite(config.GetConnectionString("default"), opt =>
            {
                opt.MigrationsAssembly("api.Web");
            });
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