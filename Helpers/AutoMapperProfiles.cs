using AutoMapper;
using DatingApp_API.ApplicationExstensions;
using DatingApp_API.Entities;
using DatingApp_API.Models;
using System.Text;

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

            CreateMap<MemberEditDTO, User>();
            CreateMap<RegisterDTO, User>();
            CreateMap<string, DateOnly>().ConvertUsing(s => DateOnly.Parse(s));
            CreateMap<string, byte[]>().ConvertUsing(s => Encoding.UTF8.GetBytes(s));
            CreateMap<Message, MessageDTO>()
                .ForMember(x => x.SenderPhotoUrl, o => o.MapFrom(s => s.Sender.Photos.FirstOrDefault(x => x.IsMain )!.Url))
                .ForMember(x => x.RecipientPhotoUrl, o => o.MapFrom(r => r.Recipient.Photos.FirstOrDefault(x => x.IsMain)!.Url));
        }
    }
}
