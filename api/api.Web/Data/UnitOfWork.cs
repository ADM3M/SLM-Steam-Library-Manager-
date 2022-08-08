using api.Interfaces;
using AutoMapper;

namespace api.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly IMapper _mapper;
    private readonly DataContext _context;
    private readonly GamesRepository _gamesRepo;

    public UnitOfWork(IMapper mapper, DataContext context)
    {
        _mapper = mapper;
        _context = context;
        _gamesRepo = new GamesRepository(context, mapper);
    }

    public IGamesRepository GamesRepo => this._gamesRepo;
    public IUserRepository UserRepo => new UserRepository(_context, _mapper, _gamesRepo);
    public IMessageRepository MessageRepo => new MessageRepository(_context, _mapper);

    public IMapper Mapper => _mapper;
    
    public async Task<bool> Complete()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    public bool HasChanges()
    {
        return _context.ChangeTracker.HasChanges();
    }
}