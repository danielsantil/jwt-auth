using AutoMapper;
using JwtAuth.Entities.Data;
using JwtAuth.Entities.ViewModels;

namespace JwtAuth
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<UserLoginViewModel, UserLogin>().ReverseMap();
        }
    }
}
