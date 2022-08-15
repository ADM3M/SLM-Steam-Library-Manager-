using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using api.Application.Interfaces;
using api.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace api.Application.Services;

public class TokenService : ITokenService
{
    private readonly UserManager<Users> _userManager;

    private readonly SymmetricSecurityKey _key;
    
    public TokenService(IConfiguration config, UserManager<Users> userManager)
    {
        _userManager = userManager;
        _key = new(Encoding.UTF8.GetBytes((string) config["TokenKey"]));
    }
    
    public async Task<string> CreateToken(Users user)
    {
        List<Claim> claims = new()
        {
            new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
        };

        var roles = await _userManager.GetRolesAsync(user);
        claims.AddRange(Enumerable.Select<string, Claim>(roles, role => new Claim(ClaimTypes.Role, role)));

        SigningCredentials creds = new(_key, SecurityAlgorithms.HmacSha512Signature);

        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(7),
            SigningCredentials = creds
        };

        JwtSecurityTokenHandler tokenHandler = new();

        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}