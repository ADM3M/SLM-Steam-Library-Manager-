using api.DTO;
using api.Entities;
using api.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace api.Data;

public class GamesRepository : IGamesRepository
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public GamesRepository(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<Games>> AddGamesAsync(List<SteamGameDTO> steamGames)
    {
        var newGames = _mapper.Map<List<Games>>(steamGames);

        await _context.Games.AddRangeAsync(newGames.ToArray());

        await _context.SaveChangesAsync();

        return newGames;
    }

    public async Task<Games> GetGameByAppSteamIdAsync(int appId)
    {
        return await _context.Games.SingleAsync(g => g.AppId == appId);
    }

    public async Task<Games> GetGameByIdAsync(int gameId)
    {
        return await _context.Games.SingleAsync(g => g.Id == gameId);
    }
    
    public async Task<Games> UpdateGameImages(int gameId, string iconUrl, string pictureUrl)
    {
        var game = await GetGameByIdAsync(gameId);
        
        if (iconUrl is not null)
        {
            game.IconUrl = iconUrl;
        }

        if (pictureUrl is not null)
        {
            game.ImageUrl = pictureUrl;
        }

        await _context.SaveChangesAsync();

        return game;
    }
    
}