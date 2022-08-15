using MediatR;

namespace api.Infrastructure.Features.Games.Commands;

public class UpdateGameImagesCommand : IRequest<Core.Entities.Games>
{
    public readonly Core.Entities.Games Game;

    public UpdateGameImagesCommand(Core.Entities.Games game)
    {
        Game = game;
    }
}