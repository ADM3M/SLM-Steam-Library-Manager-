using api.DTO;
using api.Entities;

namespace api.Interfaces;

public interface IUserRepository
{
    Task<Users> GetUserById(int userId);
    
    Task<List<UserGameDTO>> GetUserGames(int userId);

    Task<Users> UpdateUserSteamId(int userId, AccountDTO accountDto);

    Task<List<UserGameDTO>> AddGames(int userId, List<SteamGameDTO> steamGames);

    Task<UserGameDTO> UpdateGameStatus(int userId, UserGameDTO gameData);

    Task<List<string>> GetGameNames(int userId);
}