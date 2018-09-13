using AutoMapper;
using JwtAuthModels.Data;
using JwtAuthModels.ViewModels;

namespace JwtAuth
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<UserViewModel, User>().ReverseMap();
        }
    }
}
