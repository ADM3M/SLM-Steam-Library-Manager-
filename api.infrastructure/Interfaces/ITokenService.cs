using api.core.Entities;

namespace api.infrastructure.Interfaces;

public interface ITokenService
{
    Task<string> CreateToken(Users user);
}