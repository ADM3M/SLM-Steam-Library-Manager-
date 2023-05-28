using api.Core.Interfaces;
using AutoMapper;

namespace api.Application.Interfaces;

public interface IUnitOfWork
{
    IMapper Mapper { get; }
    
    IUserRepository UserRepo { get; }
    
    IGamesRepository GamesRepo { get; }
    
    IMessageRepository MessageRepo { get; }

    Task<bool> Complete();

    bool HasChanges();
    
}