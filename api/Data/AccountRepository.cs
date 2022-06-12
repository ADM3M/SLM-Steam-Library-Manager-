using api.DTO;
using api.Entities;
using api.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace api.Data;

public class AccountRepository : IAccountRepository
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    private readonly ITokenService _tokenService;

    public AccountRepository(DataContext context, IMapper mapper, ITokenService tokenService)
    {
        _context = context;
        _mapper = mapper;
        _tokenService = tokenService;
    }
    
    public async Task<UserDTO> CreateUserAsync(UserBaseDataDTO userBaseDataDto)
    {
        var newUser = _mapper.Map<Users>(userBaseDataDto);

        await _context.Users.AddAsync(newUser);

        await _context.SaveChangesAsync();

        var token = _tokenService.CreateToken(newUser);

        var userDto = _mapper.Map<UserDTO>(userBaseDataDto);
        userDto.Token = token;

        return userDto;
    }

    public async Task<UserDTO> LoginUser(UserBaseDataDTO userBaseDataDto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userBaseDataDto.UserName
            && u.Password == userBaseDataDto.Password);

        var userDto = _mapper.Map<UserDTO>(user);
        userDto.Token = _tokenService.CreateToken(user);

        return userDto;
    }

    public async Task<int> DeleteUserAsync(int userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        
        _context.Users.Remove(user);
        return await _context.SaveChangesAsync();
    }
}