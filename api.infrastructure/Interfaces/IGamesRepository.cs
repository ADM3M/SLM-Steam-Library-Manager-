using api.common.DTO;
using api.core.Entities;

namespace api.infrastructure.Interfaces;

public interface IGamesRepository
{
    Task<List<Games>> AddGamesAsync(List<SteamGameDTO> steamGames);

    Task<Games> UpdateGameImages(int appId, string iconUrl, string imageUrl);
}