using api.Application.Interfaces;
using api.Common.DTO;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

public class AccountController : BaseController
{
    private readonly IAccountRepository _accountRepository;

    public AccountController(ITokenService tokenService, IMapper mapper, IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AccountDTO>> Register([FromBody]UserAuthDataDTO userAuthDataDto)
    {
        var result = await _accountRepository.CreateUserAsync(userAuthDataDto);

        if (result is null)
        {
            return BadRequest("username is taken");
        }

        return result;
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