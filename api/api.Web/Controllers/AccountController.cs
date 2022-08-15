using api.Application.Interfaces;
using api.Common.DTO;
using api.Infrastructure.Features.Accounts.Commands;
using api.Infrastructure.Features.Accounts.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

public class AccountController : BaseController
{
    private readonly IMediator _mediator;

    public AccountController(IAccountRepository accountRepository, IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AccountDTO>> Register([FromBody]UserAuthDataDTO userAuthDataDto)
    {
        var command = new RegisterUserCommand(userAuthDataDto);
        var result = await _mediator.Send(command);

        if (result is null)
        {
            return BadRequest("username is taken");
        }

        return result;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AccountDTO>> Login([FromBody] UserAuthDataDTO userAuthDataDto)
    {
        var query = new LoginUserQuery(userAuthDataDto);
        var user = await _mediator.Send(query);

        if (user is null)
        {
            return BadRequest("invalid username or password");
        }
        
        return Ok(user);
    }
}