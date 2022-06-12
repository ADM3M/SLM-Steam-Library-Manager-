using api.DTO;
using api.Entities;
using api.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace api.Data;

public class UserRepository : IUserRepository
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public UserRepository(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Users> GetUserById(int userId)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
    }
    
    public async Task<List<UserGameDTO>> GetUserGames(int userId)
    {
        return await _context.UserGames
            .Where(ug => ug.UserId == userId)
            .Select(game => _mapper.Map<UserGameDTO>(game))
            .ToListAsync();
    }

    public async Task<Users> UpdateUserSteamId(int userId, UserDTO userDto)
    {
        var user = await GetUserById(userId);

        user.SteamId = userDto.SteamId;
        user.PhotoUrl = userDto.PhotoUrl;
        await _context.SaveChangesAsync();

        return user;
    }
    

    public async Task<UserGameDTO> AddGames(int userId, List<SteamGameDTO> list)
    {
        var user = await GetUserById(userId);
        
        foreach (SteamGameDTO item in list)
        {
            
        }

        throw new NotImplementedException();
    }
}