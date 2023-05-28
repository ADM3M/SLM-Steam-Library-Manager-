using api.Common.DTO;
using api.Core.Entities;

namespace api.Core.Interfaces;

public interface IGamesRepository
{
    Task<List<Games>> AddGamesAsync(List<SteamGameDTO> steamGames);

    Task<Games> UpdateGameImages(int appId, string iconUrl, string imageUrl);
}