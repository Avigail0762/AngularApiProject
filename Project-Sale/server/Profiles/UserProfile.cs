using AutoMapper;
using server.Models;
using server.Models.DTO;

namespace server.Profiles
{
    public class UserProfile : Profile
    {

        public UserProfile()
        {

            CreateMap<UserDTO, User>();
        }

    }
}
