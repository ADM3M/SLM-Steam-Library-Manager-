using api.Application.Interfaces;
using api.Infrastructure.Features.Games.Commands;
using MediatR;

namespace api.Infrastructure.Features.Games.Handlers;

public class AddGameHandler : IRequestHandler<AddGameCommand, List<Core.Entities.Games>>
{

    private readonly IGamesRepository _gamesRepo;

    public AddGameHandler(IGamesRepository gamesRepo)
    {
        _gamesRepo = gamesRepo;
    }

    public async Task<List<Core.Entities.Games>> Handle(AddGameCommand request, CancellationToken cancellationToken)
    {
        return await _gamesRepo.AddGamesAsync(request.SteamGames);
    }
}