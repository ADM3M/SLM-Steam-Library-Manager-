using api.Application.Interfaces;
using api.Common.DTO;
using api.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Authorize]
public class GamesController : BaseController
{
    private readonly IGamesRepository _gamesRepository;

    public GamesController(IGamesRepository gamesRepository)
    {
        _gamesRepository = gamesRepository;
    }

    [Authorize(Policy = "requireAdmin")]
    [HttpPost("addGames")]
    public async Task<ActionResult<List<SteamGameDTO>>> AddGame (List<SteamGameDTO> steamGame)
    {
        var entry =  await _gamesRepository.AddGamesAsync(steamGame);
        
        return Ok(steamGame);
    }

    [HttpPut("update")]
    public async Task<ActionResult<Games>> UpdateGameImages(Games game)
    {
        return await _gamesRepository.UpdateGameImages(game.AppId, game.IconUrl, game.ImageUrl);
    }
}