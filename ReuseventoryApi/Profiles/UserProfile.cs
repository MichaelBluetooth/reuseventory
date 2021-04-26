using AutoMapper;
using ReuseventoryApi.Models;
using ReuseventoryApi.Models.DTO;

namespace ReuseventoryApi.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserDTO, User>();
            CreateMap<User, UserDTO>();

            CreateMap<UserUpdate, User>()
                .ForMember(u => u.password, opts => opts.Ignore()); //specialized logic required for password
        }
    }
}