using api.DTO;
using api.Extensions;
using api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Authorize]
public class UserController : BaseController
{
    private readonly IUserRepository _userRepository;


    public UserController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserGameDTO>>> GetUserGames()
    {
        return await _userRepository.GetUserGames(User.GetUserId());
    }
    
    // [HttpPost]
    // public async Task AddGames(List<UserGameDTO>)

}