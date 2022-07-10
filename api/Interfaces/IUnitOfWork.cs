using AutoMapper;

namespace api.Interfaces;

public interface IUnitOfWork
{
    IMapper Mapper { get; }
    
    IUserRepository UserRepo { get; }
    
    IGamesRepository GamesRepo { get; }
    
    IMessageRepository MessageRepo { get; }

    Task<bool> Complete();

    bool HasChanges();
    
}