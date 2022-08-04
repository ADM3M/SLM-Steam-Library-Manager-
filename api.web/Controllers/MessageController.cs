using api.common.DTO;
using api.core.Entities;
using api.Extensions;
using api.infrastructure.Helpers;
using api.infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Authorize]
public class MessageController : BaseController
{
    private readonly IUnitOfWork _unit;

    public MessageController(IUnitOfWork unit)
    {
        _unit = unit;
    }

    [HttpPost]
    public async Task<ActionResult<MessageDTO>> CreateMessage(CreateMessageDTO createMessageDto)
    {
        var username = User.GetUserName();

        if (username == createMessageDto.RecipientName.ToLower())
        {
            return BadRequest("You can't send messages to yourself");
        }

        var sender = await _unit.UserRepo.GetUserByUsernameAsync(username);
        var recipient = await _unit.UserRepo.GetUserByUsernameAsync(createMessageDto.RecipientName);

        if (recipient is null)
        {
            return NotFound();
        }

        var message = new Messages
        {
            Sender = sender,
            Recipient = recipient,
            SenderName = sender.UserName,
            RecipientName = recipient.UserName,
            Content = createMessageDto.Content,
        };

        _unit.MessageRepo.AddMessage(message);

        if (await _unit.Complete())
        {
            return Ok(_unit.Mapper.Map<MessageDTO>(message));
        }

        return BadRequest("Failed to send message");
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
    {
        messageParams.Name = User.GetUserName();

        var messages = await _unit.MessageRepo.GetMessagesForUser(messageParams);

        return messages;
    }
    
    [HttpGet("thread/{username}")]
    public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessagesThread(string username)
    {
        var currentUser = User.GetUserName();

        if (username == currentUser) return BadRequest("You can't see message thread for yourself");

        return Ok(await _unit.MessageRepo.GetMessageThread(currentUser, username));
    }

    [Authorize(Policy = "requireAdmin")]
    [HttpDelete]
    public async Task<ActionResult> DeleteMessage(int messageId)
    {
        var message = await _unit.MessageRepo.GetMessage(messageId);

        if (message is null)
        {
            return BadRequest("message doesn't exists");
        }
        
        _unit.MessageRepo.DeleteMessage(message);
        if (await _unit.Complete())
        {
            return Ok();
        }

        return BadRequest("something gone wrong while deleting message");
    }
}