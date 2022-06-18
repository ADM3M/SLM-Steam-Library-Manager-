using api.DTO;
using api.Entities;
using api.Enums;
using api.Extensions;
using api.Helpers;
using api.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace api.Data;

public class UserRepository : IUserRepository
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    private readonly IGamesRepository _gameRepository;

    public UserRepository(DataContext context, IMapper mapper, IGamesRepository gameRepository)
    {
        _context = context;
        _mapper = mapper;
        _gameRepository = gameRepository;
    }

    public async Task<Users> GetUserById(int userId)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
    }
    public async Task<PagedList<UserGameDTO>> GetUserGames(int userId, DisplayParams dp)
    {
        var query = _context.UserGames
            .Where(u => u.UserId == userId)
            .AsQueryable()
            .OrderSwitch(dp); //Extension method

        if (dp.Search is not null && dp.Search.Length > 0)
        {
            query = query.Where(g => EF.Functions.Like(g.Game.Name, $"%{dp.Search}%"));
        }

        return await PagedList<UserGameDTO>.CreateAsync(
            query.ProjectTo<UserGameDTO>(_mapper.ConfigurationProvider)
                .AsNoTracking(), 
            dp.PageNumber, dp.PageSize);
    }

    public async Task<Users> UpdateUserSteamId(int userId, AccountDTO accountDto)
    {
        var user = await GetUserById(userId);

        user.SteamId = accountDto.SteamId;
        user.PhotoUrl = accountDto.PhotoUrl;
        await _context.SaveChangesAsync();

        return user;
    }
    
    // Returns List with items that are new for bd.
    private List<SteamGameDTO> GetNewGames(List<SteamGameDTO> gamesList)
    {
        var noDbGames = gamesList
            .Where(sg => !_context.Games.Any(g => g.AppId == sg.AppId))
            .ToList();
        
        return noDbGames;
    }
    
    public async Task<List<UserGameDTO>> AddGames(int userId, List<SteamGameDTO> steamGames)
    {
        var userEnt = await GetUserById(userId);

        var noDbGames = GetNewGames(steamGames);
        if (noDbGames.Any())
        {
            await _gameRepository.AddGamesAsync(noDbGames);
        }

        var gamesEnt = steamGames.Select(g =>
        {
            Games game = _context.Games.FirstOrDefault(d => d.AppId == g.AppId);
            return new UserGames(userEnt, game, g.Playtime_forever, GameStatus.NotSet);
        }).ToList();

        await _context.UserGames.AddRangeAsync(gamesEnt);
        
        await _context.SaveChangesAsync();

        return _mapper.Map<List<UserGameDTO>>(gamesEnt);
    }

    public async Task<UserGameDTO> UpdateGameStatus(int userId, UserGameDTO gameData)
    {
        var userGameEntry = await _context.UserGames
            .FirstAsync(ug => ug.GameId == gameData.GameId && ug.UserId == userId);

        userGameEntry.Status = gameData.Status;

        await _context.SaveChangesAsync();

        return gameData;
    }

    public async Task<List<string>> GetGamesName(int userId)
    {
        return await _context.UserGames
            .Where(ug => ug.UserId == userId)
            .Select(e => e.Game.Name)
            .ToListAsync();
    }
}