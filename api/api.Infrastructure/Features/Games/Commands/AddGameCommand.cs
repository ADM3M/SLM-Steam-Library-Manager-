using api.Common.DTO;
using MediatR;

namespace api.Infrastructure.Features.Games.Commands;

public class AddGameCommand : IRequest<List<Core.Entities.Games>>
{
    public readonly List<SteamGameDTO> SteamGames;

    public AddGameCommand(List<SteamGameDTO> steamGames)
    {
        SteamGames = steamGames;
    }
}