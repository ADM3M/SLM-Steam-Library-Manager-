using api.DTO;
using api.Entities;
using api.Helpers;

namespace api.Interfaces;

public interface IMessageRepository
{
    void AddGroup(Group group);

    void RemoveConnection(Connection connection);

    Task<Connection> GetConnection(string connectionId);

    Task<Group> GetMessageGroup(string groupName);

    Task<Group> GetGroupForConnection(string connectionId);
        
    void AddMessage(Messages message);

    void DeleteMessage(Messages message);

    Task<Messages> GetMessage(int id);

    Task<List<MessageDTO>> GetMessagesForUser(MessageParams messageParams);

    Task<IEnumerable<MessageDTO>> GetMessageThread(string currentName, string recipientName);
}