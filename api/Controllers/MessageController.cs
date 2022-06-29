using api.DTO;
using api.Entities;
using api.Extensions;
using api.Helpers;
using api.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Authorize]
public class MessageController : BaseController
{
    private readonly IMessageRepository _messageRepo;
    private readonly IUserRepository _userRepo;
    private readonly IMapper _mapper;

    public MessageController(IMessageRepository messageRepo, IUserRepository userRepo, IMapper mapper)
    {
        _messageRepo = messageRepo;
        _userRepo = userRepo;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<ActionResult<MessageDTO>> CreateMessage(CreateMessageDTO createMessageDto)
    {
        var username = User.GetUserName();

        if (username == createMessageDto.RecipientName.ToLower())
        {
            return BadRequest("You can't send messages to yourself");
        }

        var sender = await _userRepo.GetUserByUsernameAsync(username);
        var recipient = await _userRepo.GetUserByUsernameAsync(createMessageDto.RecipientName);

        if (recipient is null)
        {
            return NotFound();
        }

        var message = new Messages()
        {
            Sender = sender,
            Recipient = recipient,
            SenderName = sender.UserName,
            RecipientName = recipient.UserName,
            Content = createMessageDto.Content,
        };

        _messageRepo.AddMessage(message);

        if (await _messageRepo.SaveAllAsync())
        {
            return Ok(_mapper.Map<MessageDTO>(message));
        }

        return BadRequest("Failed to send message");
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
    {
        messageParams.Name = User.GetUserName();

        var messages = await _messageRepo.GetMessagesForUser(messageParams);

        return messages;
    }
    
    [HttpGet("thread/{username}")]
    public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessagesThread(string username)
    {
        var currentUser = User.GetUserName();

        if (username == currentUser) return BadRequest("You can't see message thread for yourself");

        return Ok(await _messageRepo.GetMessageThread(currentName:  currentUser, recipientName: username));
    }
}