using api.Application.Helpers;
using api.Application.Interfaces;
using api.Common.DTO;
using api.Common.Enums;
using api.Core.Entities;
using api.Core.Interfaces;
using api.Infrastructure.Extensions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace api.Infrastructure.Data;

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

    public async Task<Users> GetUserByIdAsync(int userId)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<Users> GetUserByUsernameAsync(string username)
    {
        return await _context.Users
            .SingleOrDefaultAsync(user => user.UserName == username);
    }
    public async Task<PagedList> GetUserGames(int userId, DisplayParams dp)
    {
        var query = Queryable.Where<UserGames>(_context.UserGames, u => u.UserId == userId)
            .AsQueryable();

        if (dp.PageNumber != -1)
        {
            if (dp.Search is not null && dp.Search.Length > 0)
            {
                query = Queryable.Where(query, g => EF.Functions.Like(g.Game.Name, $"%{dp.Search}%"));
            }

            query = query.OrderSwitch(dp); //Extension method
        }

        return PagedList.Create(
            Enumerable.ToList<UserGameDTO>(query.ProjectTo<UserGameDTO>(_mapper.ConfigurationProvider).AsNoTracking()), dp);
    }

    public async Task<Users> UpdateUserSteamId(int userId, AccountDTO accountDto)
    {
        var user = await GetUserByIdAsync(userId);

        user.SteamId = accountDto.SteamId;
        user.PhotoUrl = accountDto.PhotoUrl;

        var userGames = await _context.UserGames.Where(ug => ug.UserId == user.Id).ToListAsync();
        _context.RemoveRange(userGames);

        return user;
    }
    
    // Returns List with items that are new for bd.
    public List<SteamGameDTO> GetNewGames(List<SteamGameDTO> gamesList, List<Games> sourceList)
    {
        var noDbGames = gamesList
            .Where(sg => !sourceList.Any(g => g.AppId == sg.AppId))
            .ToList();
        
        return noDbGames;
    }
    
    public async Task<List<UserGameDTO>> AddGames(int userId, List<SteamGameDTO> steamGames)
    {
        var userEnt = await GetUserByIdAsync(userId);

        var noDbGames = GetNewGames(steamGames, Enumerable.ToList<Games>(this._context.Games));
        if (noDbGames.Any())
        {
            await _gameRepository.AddGamesAsync(noDbGames);
        }

        var gamesEnt = steamGames.Select(g =>
        {
            Games game = _context.Games.FirstOrDefault(d => d.AppId == g.AppId);
            return new UserGames(userEnt, game, g.Playtime_forever, g.Playtime_forever > 0 ? GameStatus.InProgress : GameStatus.NotSet );
        }).ToList();

        await _context.UserGames.AddRangeAsync(gamesEnt);

        return _mapper.Map<List<UserGameDTO>>(gamesEnt);
    }

    public async Task<UserGameDTO> UpdateGameStatus(int userId, UserGameDTO gameData)
    {
        var userGameEntry = await _context.UserGames
            .FirstAsync(ug => ug.GameId == gameData.GameId && ug.UserId == userId);

        userGameEntry.Status = gameData.Status;

        return gameData;
    }

    public async Task<List<string>> GetGamesName(int userId)
    {
        return await Queryable.Where<UserGames>(_context.UserGames, ug => ug.UserId == userId)
            .Select(e => e.Game.Name)
            .ToListAsync();
    }
}