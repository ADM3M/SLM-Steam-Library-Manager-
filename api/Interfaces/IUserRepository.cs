using api.DTO;
using api.Entities;
using api.Helpers;

namespace api.Interfaces;

public interface IUserRepository
{
    Task<Users> GetUserByIdAsync(int userId);

    Task<Users> GetUserByUsernameAsync(string username);
    
    Task<PagedList> GetUserGames(int userId, DisplayParams dp);

    Task<Users> UpdateUserSteamId(int userId, AccountDTO accountDto);

    Task<List<UserGameDTO>> AddGames(int userId, List<SteamGameDTO> steamGames);

    Task<UserGameDTO> UpdateGameStatus(int userId, UserGameDTO gameData);

    Task<List<string>> GetGamesName(int userId);
}