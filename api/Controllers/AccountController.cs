using api.Data;
using api.DTO;
using api.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers;

public class AccountController : BaseController
{
    private readonly IAccountRepository _accountRepository;
    private readonly DataContext _context;

    public AccountController(ITokenService tokenService, IMapper mapper, IAccountRepository accountRepository, DataContext context)
    {
        _accountRepository = accountRepository;
        _context = context;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDTO>> Register([FromBody]UserBaseDataDTO userBaseDataDto)
    {
        if (await _context.Users.AnyAsync(u => u.UserName == userBaseDataDto.UserName))
        {
            return BadRequest("username is taken");
        }
        
        return await _accountRepository.CreateUserAsync(userBaseDataDto);
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDTO>> Login([FromBody] UserBaseDataDTO userBaseDataDto)
    {
        var user = await _accountRepository.LoginUser(userBaseDataDto);

        if (user is null)
        {
            return BadRequest("invalid username or password");
        }

        
        return Ok(user);
    }
}