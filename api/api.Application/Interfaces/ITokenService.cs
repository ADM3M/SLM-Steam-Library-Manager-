using api.Core.Entities;

namespace api.Application.Interfaces;

public interface ITokenService
{
    Task<string> CreateToken(Users user);
}