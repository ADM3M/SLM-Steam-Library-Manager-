using api.Application.Interfaces;
using api.Common.DTO;
using api.Core.Entities;
using api.Infrastructure.Features.Games.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Authorize]
public class GamesController : BaseController
{
    private readonly IMediator _mediator;

    public GamesController(IGamesRepository gamesRepository, IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Policy = "requireAdmin")]
    [HttpPost("addGames")]
    public async Task<ActionResult<List<SteamGameDTO>>> AddGame (List<SteamGameDTO> steamGame)
    {
        var command = new AddGameCommand(steamGame);
        await _mediator.Send(command);
        
        return Ok(steamGame);
    }

    [HttpPut("update")]
    public async Task<ActionResult<Games>> UpdateGameImages(Games game)
    {
        var command = new UpdateGameImagesCommand(game);
        return await _mediator.Send(command);
    }
}