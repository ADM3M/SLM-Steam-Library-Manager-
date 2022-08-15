using api.Application.Interfaces;
using api.Infrastructure.Features.Games.Commands;
using MediatR;

namespace api.Infrastructure.Features.Games.Handlers;

public class UpdateGameImagesHandler : IRequestHandler<UpdateGameImagesCommand, Core.Entities.Games>
{
    private readonly IUnitOfWork _uow;

    public UpdateGameImagesHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }
    
    public async Task<Core.Entities.Games> Handle(UpdateGameImagesCommand request, CancellationToken cancellationToken)
    {
        return await _uow.GamesRepo.UpdateGameImages(
            request.Game.AppId, 
            request.Game.IconUrl, 
            request.Game.ImageUrl);
    }
}