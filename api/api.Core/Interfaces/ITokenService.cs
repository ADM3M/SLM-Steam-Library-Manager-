using api.Core.Entities;

namespace api.Core.Interfaces;

public interface ITokenService
{
    Task<string> CreateToken(Users user);
}