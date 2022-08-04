using api.common.DTO;
using api.core.Entities;
using api.infrastructure.Helpers;

namespace api.infrastructure.Interfaces;

public interface IMessageRepository
{
    void AddGroup(core.Entities.Group group);

    Task<Connection> GetConnection(string connectionId);

    Task<core.Entities.Group> GetMessageGroup(string groupName);

    Task<core.Entities.Group> GetGroupForConnection(string connectionId);
        
    void AddMessage(Messages message);

    void DeleteMessage(Messages message);

    Task<Messages> GetMessage(int id);

    Task<List<MessageDTO>> GetMessagesForUser(MessageParams messageParams);

    Task<IEnumerable<MessageDTO>> GetMessageThread(string currentName, string recipientName);

    void RemoveConnection(Connection connection);
}