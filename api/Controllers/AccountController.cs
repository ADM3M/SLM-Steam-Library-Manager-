using api.Data;
using api.DTO;
using api.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers;

public class AccountController : BaseController
{
    private readonly IUserRepository _userRepository;
    private readonly DataContext _context;

    public AccountController(ITokenService tokenService, IMapper mapper, IUserRepository userRepository, DataContext context)
    {
        _userRepository = userRepository;
        _context = context;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDTO>> Register([FromBody]UserBaseDataDTO userBaseDataDto)
    {
        if (await _context.Users.AnyAsync(u => u.UserName == userBaseDataDto.UserName))
        {
            return BadRequest("username is taken");
        }
        
        return await _userRepository.CreateUserAsync(userBaseDataDto);
    }

    [HttpGet("login")]
    public ActionResult<UserDTO> Login([FromBody] UserBaseDataDTO userBaseDataDto)
    {
        var user = _userRepository.LoginUser(userBaseDataDto);

        if (user is null)
        {
            return BadRequest("invalid username or password");
        }

        
        return user;
    }
}