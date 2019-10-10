using System.Linq;
using AutoMapper;
using DatingApp.API.Dtos;
using DatingApp.API.Models;

namespace DatingApp.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserListDto>()
            .ForMember(dest => dest.PhotoUrl, opt =>
            {
                opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain == true).Url);
            })
            .ForMember(dest => dest.Age, opt =>
             {
                 opt.ResolveUsing(src => src.DateOfBirth.CalculateAge());
             });
            CreateMap<User, UserDetailsDto>()
             .ForMember(dest => dest.PhotoUrl, opt =>
            {
                opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain == true).Url);
            }).ForMember(dest => dest.Age, opt =>
            {
                opt.ResolveUsing(src => src.DateOfBirth.CalculateAge());
            });
            CreateMap<Photo, PhotoDetailsDto>();
            CreateMap<PhotoCreateDto, Photo>();
            CreateMap<Photo, PhotoToReturnDto>();
            CreateMap<UserUpdateDto, User>();
            CreateMap<UserRegisterDto, User>();
        }
    }
}