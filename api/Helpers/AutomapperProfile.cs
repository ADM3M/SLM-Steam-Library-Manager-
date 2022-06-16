using api.DTO;
using api.Entities;
using AutoMapper;

namespace api.Helpers;

public class AutomapperProfile : Profile
{
    public AutomapperProfile()
    {
        CreateMap<UserAuthDataDTO, AccountDTO>();
        
        CreateMap<UserAuthDataDTO, Users>();

        CreateMap<Users, AccountDTO>();

        CreateMap<UserGames, UserGameDTO>()
            .ForMember(dest => dest.AppId, opt => opt.MapFrom(src => src.Game.AppId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Game.Name))
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Game.ImageUrl))
            .ForMember(dest => dest.IconUrl, opt => opt.MapFrom(src => src.Game.IconUrl));

        CreateMap<SteamGameDTO, Games>()
            .ForMember(dest => dest.IconUrl, opt => opt.MapFrom(src => src.Img_icon_url));

    }
}