using api.DTO;
using api.Entities;
using api.Helpers;

namespace api.Interfaces;

public interface IUserRepository
{
    Task<Users> GetUserById(int userId);
    
    Task<PagedList<UserGameDTO>> GetUserGames(int userId, PaginationParams pag);

    Task<Users> UpdateUserSteamId(int userId, AccountDTO accountDto);

    Task<List<UserGameDTO>> AddGames(int userId, List<SteamGameDTO> steamGames);

    Task<UserGameDTO> UpdateGameStatus(int userId, UserGameDTO gameData);

    Task<List<string>> GetGamesName(int userId);
}