using api.DTO;
using api.Entities;
using api.Extensions;
using api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Authorize]
public class UserController : BaseController
{
    private readonly IUserRepository _userRepository;


    public UserController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserGameDTO>>> GetUserGames()
    {
        return Ok(await _userRepository.GetUserGames(User.GetUserId()));
    }

    [HttpPost("addGames")]
    public async Task<ActionResult<List<UserGameDTO>>> AddGames(List<SteamGameDTO> steamGames)
    {
        return Ok(await _userRepository.AddGames(User.GetUserId(), steamGames));
    }

    [HttpPut("updateSteamId")]
    public async Task<ActionResult<Users>> UpdateSteamId(string steamId, string? photoUrl)
    {
        return await _userRepository.UpdateUserSteamId(User.GetUserId(), new AccountDTO{SteamId = steamId, PhotoUrl = photoUrl});
    }

    [HttpPut("updateGameStatus")]
    public async Task<ActionResult<UserGameDTO>> UpdateGameStatus([FromBody] UserGameDTO gameData)
    {
        return await _userRepository.UpdateGameStatus(User.GetUserId(), gameData);
    }

    [HttpGet("getGamesName")]
    public async Task<List<string>> GetGamesName()
    {
        return await _userRepository.GetGamesName(User.GetUserId());
    }

}