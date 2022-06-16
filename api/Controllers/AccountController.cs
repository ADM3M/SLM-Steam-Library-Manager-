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
    public async Task<ActionResult<AccountDTO>> Register([FromBody]UserAuthDataDTO userAuthDataDto)
    {
        if (await _context.Users.AnyAsync(u => u.UserName == userAuthDataDto.UserName))
        {
            return BadRequest("username is taken");
        }
        
        return await _accountRepository.CreateUserAsync(userAuthDataDto);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AccountDTO>> Login([FromBody] UserAuthDataDTO userAuthDataDto)
    {
        var user = await _accountRepository.LoginUser(userAuthDataDto);

        if (user is null)
        {
            return BadRequest("invalid username or password");
        }

        
        return Ok(user);
    }
}