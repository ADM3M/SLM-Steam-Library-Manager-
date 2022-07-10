using api.DTO;
using api.Entities;
using api.Extensions;
using api.Interfaces;
using API.SignalR;
using Microsoft.AspNetCore.SignalR;

namespace api.SignalR;

public class MessageHub : Hub
{
    private readonly IUnitOfWork _unit;
    private readonly PresenceTracker _tracker;
    private readonly IHubContext<PresenceHub> _presenceHub;

    public MessageHub(IUnitOfWork unit,
        PresenceTracker tracker,
        IHubContext<PresenceHub> presenceHub)
    {
        _unit = unit;
        _tracker = tracker;
        _presenceHub = presenceHub;
    }

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var otherUser = httpContext.Request.Query["user"].ToString();
        var groupName = GetGroupName(Context.User.GetUserName(), otherUser);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        var group = await AddToGroup(groupName);

        await Clients.Group(groupName).SendAsync("UpdatedGroup", group);

        var messages = await _unit.MessageRepo
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

        var sender = await _unit.UserRepo.GetUserByUsernameAsync(username);
        var recipient = await _unit.UserRepo.GetUserByUsernameAsync(messageDTO.RecipientName);

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

        var group = await _unit.MessageRepo.GetMessageGroup(groupName);

        if (!group.Connections.Any(x => x.UserName == recipient.UserName))
        {
            var connections  = await _tracker.GetConnectionsForUser(recipient.UserName);
            if (connections is not null)
            {
                await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived", 
                    new {username = sender.UserName, knownAs = sender.UserName});
            }
        }

        _unit.MessageRepo.AddMessage(message);

        if (await _unit.Complete())
        {
            await Clients.Group(groupName).SendAsync("NewMessage", _unit.Mapper.Map<MessageDTO>(message));
        }
    }

    private async Task<Group> AddToGroup(string groupName)
    {
        var group = await _unit.MessageRepo.GetMessageGroup(groupName);
        var connection = new Connection(Context.ConnectionId, Context.User.GetUserName());

        if (group is null)
        {
            group = new Group(groupName);
            _unit.MessageRepo.AddGroup(group);
        }

        group.Connections.Add(connection);

        if (await _unit.Complete())
        {
            return group;
        }

        throw new HubException("Failed to join group");
    }

    private async Task<Group> RemoveFromMessageGroup()
    {
        var group = await _unit.MessageRepo.GetGroupForConnection(Context.ConnectionId);
        var connection = group.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
        _unit.MessageRepo.RemoveConnection(connection);
        if (await _unit.Complete())
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