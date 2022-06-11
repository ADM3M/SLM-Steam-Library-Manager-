using api.DTO;
using api.Entities;
using AutoMapper;

namespace api.Helpers;

public class AutomapperProfile : Profile
{
    public AutomapperProfile()
    {
        CreateMap<UserBaseDataDTO, UserDTO>();
        
        CreateMap<UserBaseDataDTO, Users>();

        CreateMap<Users, UserDTO>();
    }
}