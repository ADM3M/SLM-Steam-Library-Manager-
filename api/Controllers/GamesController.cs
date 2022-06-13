using api.DTO;
using api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

public class GamesController : BaseController
{
    private readonly IGamesRepository _gamesRepository;

    public GamesController(IGamesRepository gamesRepository)
    {
        _gamesRepository = gamesRepository;
    }

    [HttpPost("addGames")]
    public async Task<ActionResult<List<SteamGameDTO>>> AddGame (List<SteamGameDTO> steamGame)
    {
        var entry =  await _gamesRepository.AddGamesAsync(steamGame);
        
        return Ok(steamGame);
    }
}