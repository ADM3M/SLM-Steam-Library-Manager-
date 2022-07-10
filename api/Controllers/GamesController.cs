using api.DTO;
using api.Entities;
using api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Authorize]
public class GamesController : BaseController
{
    private readonly IUnitOfWork _unit;

    public GamesController(IUnitOfWork unit)
    {
        _unit = unit;
    }

    [Authorize(Policy = "requireAdmin")]
    [HttpPost("addGames")]
    public async Task<ActionResult<List<SteamGameDTO>>> AddGame (List<SteamGameDTO> steamGame)
    {
        var entry =  await _unit.GamesRepo.AddGamesAsync(steamGame);
        
        return Ok(steamGame);
    }

    [HttpPut("update")]
    public async Task<ActionResult<Games>> UpdateGameImages(Games game)
    {
        var updateGame = await _unit.GamesRepo.UpdateGameImages(game.AppId, game.IconUrl, game.ImageUrl);

        if (await _unit.Complete()) return Ok(updateGame);

        return BadRequest("Error acquired during game image");
    }
}