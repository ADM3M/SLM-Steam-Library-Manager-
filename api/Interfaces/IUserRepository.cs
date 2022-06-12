using api.DTO;
using api.Entities;

namespace api.Interfaces;

public interface IUserRepository
{
    Task<Users> GetUserById(int userId);
    
    Task<List<UserGameDTO>> GetUserGames(int userId);

    Task<Users> UpdateUserSteamId(int userId, UserDTO userDTO);

    Task<UserGameDTO> AddGames(int userId, List<UserGameDTO> list);
}