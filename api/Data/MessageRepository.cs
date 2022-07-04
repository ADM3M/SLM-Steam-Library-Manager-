using api.DTO;
using api.Entities;
using api.Helpers;
using api.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace api.Data;

public class MessageRepository : IMessageRepository
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public MessageRepository(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public void AddGroup(Group group)
    {
        _context.Groups.Add(group);
    }

    public void RemoveConnection(Connection connection)
    {
        throw new NotImplementedException();
    }

    public async Task<Connection> GetConnection(string connectionId)
    {
        return await _context.Connections.FindAsync(connectionId);
    }

    public async Task<Group> GetMessageGroup(string groupName)
    {
        return await _context.Groups
            .Include(x => x.Connections)
            .FirstOrDefaultAsync(x => x.Name == groupName);
    }

    public async Task<Group> GetGroupForConnection(string connectionId)
    {
        return await _context.Groups
            .Include(c => c.Connections)
            .Where(c => c.Connections.Any(x => x.ConnectionId == connectionId))
            .FirstOrDefaultAsync();
    }

    public void AddMessage(Messages message)
    {
        _context.Messages.Add(message);
    }

    public void DeleteMessage(Messages message)
    {
        _context.Messages.Remove(message);
    }

    public async Task<Messages> GetMessage(int id)
    {
        return await _context.Messages.FindAsync(id);
    }

    public async Task<Messages> GetMessageAsync(int id)
    {
        return await _context.Messages.FindAsync(id);
    }

    public async Task<List<MessageDTO>> GetMessagesForUser(MessageParams messageParams)
    {
        var query = _context.Messages.OrderByDescending(m => m.MessageSent)
            .ProjectTo<MessageDTO>(_mapper.ConfigurationProvider)
            .AsQueryable();
            
        query = messageParams.Container switch
        {
            "Inbox" => query.Where(u => u.RecipientName == messageParams.Name),
            "Outbox" => query.Where(u => u.SenderName == messageParams.Name),
            _ => query.Where(u => u.RecipientName == messageParams.Name)
        };

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<MessageDTO>> GetMessageThread(string currentName, string recipientName)
    {
        var messages = await _context.Messages
            .Where(m => m.SenderName == recipientName
                        && m.RecipientName == currentName
                        || m.SenderName == currentName
                        && m.RecipientName == recipientName
                        && !m.SenderDeleted
            )
            .OrderBy(m => m.MessageSent)
            .ProjectTo<MessageDTO>(_mapper.ConfigurationProvider)
            .ToListAsync();
            
        var unreadMessages = messages
            .Where(m => m.RecipientName == currentName)
            .Take(3)
            .ToList();

        return messages;
    }

    public async Task<bool> SaveAllAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}