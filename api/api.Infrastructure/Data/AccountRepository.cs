using api.Common.DTO;
using api.Core.Entities;
using api.Core.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace api.Infrastructure.Data;

public class AccountRepository : IAccountRepository
{
    private readonly IMapper _mapper;
    private readonly ITokenService _tokenService;
    private readonly UserManager<Users> _userManager;
    private readonly SignInManager<Users> _signInManager;

    public AccountRepository(IMapper mapper, ITokenService tokenService,
        UserManager<Users> userManager, SignInManager<Users> signInManager)
    {
        _mapper = mapper;
        _tokenService = tokenService;
        _userManager = userManager;
        _signInManager = signInManager;
    }
    
    public async Task<AccountDTO> CreateUserAsync(UserAuthDataDTO userAuthDataDto)
    {
        var newUser = _mapper.Map<Users>(userAuthDataDto);

        var result = await _userManager.CreateAsync(newUser, userAuthDataDto.Password);
        if (!result.Succeeded) return null;

        var roleResult = await _userManager.AddToRoleAsync(newUser, "member");
        if (!roleResult.Succeeded) return null;


        var token = await _tokenService.CreateToken(newUser);

        var accountDto = _mapper.Map<AccountDTO>(userAuthDataDto);
        accountDto.Token = token;

        return accountDto;
    }

    public async Task<AccountDTO> LoginUser(UserAuthDataDTO userAuthDataDto)
    {
        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.NormalizedUserName == userAuthDataDto.UserName.ToUpper());

        if (user is null)
        {
            return null;
        }

        var result = await _signInManager
            .CheckPasswordSignInAsync(user, userAuthDataDto.Password, false);

        if (!result.Succeeded)
        {
            return null;
        }

        var userDto = _mapper.Map<AccountDTO>(user);
        userDto.Token = await _tokenService.CreateToken(user);

        return userDto;
    }

    public async Task<bool> DeleteUserAsync(int userId)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);

        var result = await _userManager.DeleteAsync(user);

        return result.Succeeded;
    }
}