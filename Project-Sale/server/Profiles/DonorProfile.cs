using AutoMapper;
using server.Models;
using server.Models.DTO;

namespace server.Profiles
{
    public class DonorProfile: Profile
    {
        public DonorProfile()
        {
            CreateMap<DonorDTO, Donor>();
        }
    }
}
