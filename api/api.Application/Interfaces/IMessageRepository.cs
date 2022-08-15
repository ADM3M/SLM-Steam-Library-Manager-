using api.Application.Helpers;
using api.Common.DTO;
using api.Core.Entities;

namespace api.Application.Interfaces;

public interface IMessageRepository
{
    void AddGroup(Core.Entities.Group group);

    void RemoveConnection(Connection connection);

    Task<Connection> GetConnection(string connectionId);

    Task<Core.Entities.Group> GetMessageGroup(string groupName);

    Task<Core.Entities.Group> GetGroupForConnection(string connectionId);
        
    void AddMessage(Messages message);

    void DeleteMessage(Messages message);

    Task<Messages> GetMessage(int id);

    Task<List<MessageDTO>> GetMessagesForUser(MessageParams messageParams);

    Task<IEnumerable<MessageDTO>> GetMessageThread(string currentName, string recipientName);
}