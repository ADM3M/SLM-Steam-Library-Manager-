using api.DTO;
using api.Entities;
using api.Extensions;
using api.Helpers;
using api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Authorize]
public class UserController : BaseController
{
    private readonly IUnitOfWork _unit;

    public UserController(IUnitOfWork unit)
    {
        _unit = unit;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserGameDTO>>> GetUserGames([FromQuery] DisplayParams dp)
    {
        var games = await _unit.UserRepo.GetUserGames(User.GetUserId(), dp);

        Response.AddPaginationHeader(games.CurrentPage, games.PageSize, games.TotalCount, games.TotalPages);
        
        return Ok(games);
    }

    [HttpPost("addGames")]
    public async Task<ActionResult<List<UserGameDTO>>> AddGames(List<SteamGameDTO> steamGames)
    {
        var games = await _unit.UserRepo.AddGames(User.GetUserId(), steamGames);

        if (await _unit.Complete()) return Ok(games);

        return BadRequest("Error acquired during adding games");
    }

    [HttpPut("updateSteamId")]
    public async Task<ActionResult<Users>> UpdateSteamId(string steamId, string? photoUrl)
    {
        var updatedAcc = await _unit.UserRepo.UpdateUserSteamId(User.GetUserId(), 
            new AccountDTO{SteamId = steamId, PhotoUrl = photoUrl});
        
        if (await _unit.Complete()) return Ok(updatedAcc);

        return BadRequest("Error acquired during updating steamId");
    }

    [HttpPut("updateGameStatus")]
    public async Task<ActionResult<UserGameDTO>> UpdateGameStatus([FromBody] UserGameDTO gameData)
    {
        var game = await _unit.UserRepo.UpdateGameStatus(User.GetUserId(), gameData);

        if (await _unit.Complete()) return Ok(game);

        return BadRequest("Error acquired during updating game status");
    }

    [HttpGet("getGamesName")]
    public async Task<List<string>> GetGamesName()
    {
        return await _unit.UserRepo.GetGamesName(User.GetUserId());
    }

}