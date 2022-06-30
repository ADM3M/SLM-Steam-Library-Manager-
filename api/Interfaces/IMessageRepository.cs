using api.DTO;
using api.Entities;
using api.Helpers;

namespace api.Interfaces;

public interface IMessageRepository
{
        
    void AddMessage(Messages message);

    void DeleteMessage(Messages message);

    Task<Messages> GetMessageAsync(int id);

    Task<List<MessageDTO>> GetMessagesForUser(MessageParams messageParams);

    Task<IEnumerable<MessageDTO>> GetMessageThread(string currentName, string recipientName);

    Task<bool> SaveAllAsync();
}