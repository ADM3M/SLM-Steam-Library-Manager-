using api.Core.Entities;
using api.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers;

[Authorize(Policy = "requireAdmin")]
public class AdminController : BaseController
{
    private readonly DataContext _context;

    public AdminController(DataContext context)
    {
        _context = context;
    }

    private async Task<Games> getGameFromBd(int appId)
    {
        return await _context.Games.FirstOrDefaultAsync(g => g.AppId == appId);
    }
    
    [HttpPut("updateGameImg")]
    public async Task<ActionResult<Games>> UpdateGameImg(Games gameInfo)
    {
        var game = await getGameFromBd(gameInfo.AppId);
        if (game is null) return BadRequest("game not found");

        game.ImageUrl = gameInfo.ImageUrl;
        await _context.SaveChangesAsync();

        return Ok(game);
    }

    [HttpGet("getGameInfo")]
    public async Task<ActionResult<Games>> GetGameInfo(int appId)
    {
        var game= await getGameFromBd(appId);
        if (game is null) return BadRequest("game not found");

        return Ok(game);
    }
}