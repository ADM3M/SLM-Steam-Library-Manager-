using api.Data;
using api.Helpers;
using api.Interfaces;
using api.Services;
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
            .AddScoped<ITokenService, TokenService>()
            .AddScoped<IAccountRepository, AccountRepository>()
            .AddAutoMapper(typeof(AutomapperProfile).Assembly)
            .AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}