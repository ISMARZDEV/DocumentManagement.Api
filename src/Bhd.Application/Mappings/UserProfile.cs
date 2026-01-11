using AutoMapper;
using Bhd.Application.DTOs.UserDTOs;
using Bhd.Domain.Entities;

namespace Bhd.Application.Mappings;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<User, UserCreateDto>().ReverseMap();
        CreateMap<User, UserLoginDto>().ReverseMap();
        CreateMap<User, UserResponseDto>()
            .ForMember(dest => dest.Created, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.Token, opt => opt.Ignore());
    }
}