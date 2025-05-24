using AutoMapper;
using DatingApp_API.ApplicationExstensions;
using DatingApp_API.Entities;
using DatingApp_API.Models;

namespace DatingApp_API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles() 
        {
            CreateMap<User, MemberDTO>()
                .ForMember(dest => dest.Age, opt => opt.MapFrom(x => x.DateOfBirth.CalculateAge()))
                .ForMember(dest => dest.PhotoUrl,
                opt => opt.MapFrom(source => source.Photos.FirstOrDefault(x => x.IsMain)!.Url));
            CreateMap<Photo, PhotoDTO>();
        }
    }
}
