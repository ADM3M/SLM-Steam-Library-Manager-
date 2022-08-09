using api.Application.Interfaces;
using api.Infrastructure.Features.Games.Commands;
using MediatR;

namespace api.Infrastructure.Features.Games.Handlers;

public class UpdateGameImagesHandler : IRequestHandler<UpdateGameImagesCommand, Core.Entities.Games>
{
    private readonly IGamesRepository _gamesRepo;

    public UpdateGameImagesHandler(IGamesRepository gamesRepo)
    {
        this._gamesRepo = gamesRepo;
    }
    
    public async Task<Core.Entities.Games> Handle(UpdateGameImagesCommand request, CancellationToken cancellationToken)
    {
        return await _gamesRepo.UpdateGameImages(
            request.Game.AppId, 
            request.Game.IconUrl, 
            request.Game.ImageUrl);
    }
}