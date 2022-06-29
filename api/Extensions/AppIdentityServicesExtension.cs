using System.Text;
using api.Data;
using api.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace api.Extensions;

public static class AppIdentityServicesExtension
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, ConfigurationManager config)
    {
        services.AddIdentityCore<Users>(opt =>
            {
                opt.Password.RequiredLength = 4;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireNonAlphanumeric = false;
            })
            .AddRoles<AppRole>()
            .AddRoleManager<RoleManager<AppRole>>()
            .AddRoleValidator<RoleValidator<AppRole>>()
            .AddSignInManager<SignInManager<Users>>()
            .AddEntityFrameworkStores<DataContext>();
        
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"])),
                ValidateIssuer = false,
                ValidateAudience = false,
            });

        services.AddAuthorization(opt =>
        {
            opt.AddPolicy("requireAdmin", policy => policy.RequireRole("admin"));
        });
        
        return services;
    }
}