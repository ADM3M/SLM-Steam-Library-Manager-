using AutoMapper;

namespace api.infrastructure.Interfaces;

public interface IUnitOfWork
{
    IMapper Mapper { get; }
    
    IUserRepository UserRepo { get; }
    
    IGamesRepository GamesRepo { get; }
    
    IMessageRepository MessageRepo { get; }

    Task<bool> Complete();

    bool HasChanges();
    
}