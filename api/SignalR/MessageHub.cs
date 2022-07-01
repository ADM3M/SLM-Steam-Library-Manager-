using api.DTO;
using api.Entities;
using api.Extensions;
using api.Interfaces;
using API.SignalR;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace api.SignalR;

public class MessageHub : Hub
{
    private readonly IMessageRepository _messageRepository;
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;
    private readonly PresenceTracker _tracker;
    private readonly IHubContext<PresenceHub> _presenceHub;

    public MessageHub(IMessageRepository messageRepository, 
        IMapper mapper, 
        IUserRepository userRepository,
        PresenceTracker tracker,
        IHubContext<PresenceHub> presenceHub)
    {
        _userRepository = userRepository;
        _tracker = tracker;
        _presenceHub = presenceHub;
        _mapper = mapper;
        _messageRepository = messageRepository;
    }

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var otherUser = httpContext.Request.Query["user"].ToString();
        var groupName = GetGroupName(Context.User.GetUserName(), otherUser);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        var group = await AddToGroup(groupName);

        await Clients.Group(groupName).SendAsync("UpdatedGroup", group);

        var messages = await _messageRepository
            .GetMessageThread(Context.User.GetUserName(), otherUser);

        await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var group = await RemoveFromMessageGroup();
        await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(CreateMessageDTO messageDTO)
    {
        var username = Context.User.GetUserName();

        if (username == messageDTO.RecipientName.ToLower())
        {
            throw new HubException("You can't send messages to yourself");
        }

        var sender = await _userRepository.GetUserByUsernameAsync(username);
        var recipient = await _userRepository.GetUserByUsernameAsync(messageDTO.RecipientName);

        if (recipient is null)
        {
            throw new HubException("Not found user");
        }

        var message = new Messages
        {
            Sender = sender,
            Recipient = recipient,
            SenderName = sender.UserName,
            RecipientName = recipient.UserName,
            Content = messageDTO.Content,
        };

        var groupName = GetGroupName(sender.UserName, recipient.UserName);

        var group = await _messageRepository.GetMessageGroup(groupName);

        if (!group.Connections.Any(x => x.UserName == recipient.UserName))
        {
            var connections  = await _tracker.GetConnectionsForUser(recipient.UserName);
            if (connections is not null)
            {
                await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived", 
                    new {username = sender.UserName, knownAs = sender.UserName});
            }
        }

        _messageRepository.AddMessage(message);

        if (await _messageRepository.SaveAllAsync())
        {
            await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDTO>(message));
        }
    }

    private async Task<Group> AddToGroup(string groupName)
    {
        var group = await _messageRepository.GetMessageGroup(groupName);
        var connection = new Connection(Context.ConnectionId, Context.User.GetUserName());

        if (group is null)
        {
            group = new Group(groupName);
            _messageRepository.AddGroup(group);
        }

        group.Connections.Add(connection);

        if (await _messageRepository.SaveAllAsync())
        {
            return group;
        }

        throw new HubException("Failed to join group");
    }

    private async Task<Group> RemoveFromMessageGroup()
    {
        var group = await _messageRepository.GetGroupForConnection(Context.ConnectionId);
        var connection = group.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
        _messageRepository.RemoveConnection(connection);
        if (await _messageRepository.SaveAllAsync())
        {
            return group;
        }

        throw new HubException("Failed to remove from group");
    }

    private string GetGroupName(string caller, string other)
    {
        var stringCompare = string.CompareOrdinal(caller, other) < 0;
        return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
    }
}