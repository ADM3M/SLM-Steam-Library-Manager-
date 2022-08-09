using api.Application.Interfaces;
using api.Infrastructure.Features.Games.Commands;
using MediatR;

namespace api.Infrastructure.Features.Games.Handlers;

public class AddGameHandler : IRequestHandler<AddGameCommand, List<Core.Entities.Games>>
{
    private readonly IUnitOfWork _uow;

    public AddGameHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<List<Core.Entities.Games>> Handle(AddGameCommand request, CancellationToken cancellationToken)
    {
        return await _uow.GamesRepo.AddGamesAsync(request.SteamGames);
    }
}