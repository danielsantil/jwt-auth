using AutoMapper;
using JwtAuthModels.Data;
using JwtAuthModels.ViewModels;

namespace JwtAuth.Services
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<UserViewModel, User>().ReverseMap();
        }
    }
}
