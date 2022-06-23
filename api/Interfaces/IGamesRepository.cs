using api.DTO;
using api.Entities;

namespace api.Interfaces;

public interface IGamesRepository
{
    Task<List<Games>> AddGamesAsync(List<SteamGameDTO> steamGames);

    Task<Games> UpdateGameImages(int appId, string iconUrl, string imageUrl);
}