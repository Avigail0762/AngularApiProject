using AutoMapper;
using server.Bll.Interfaces;
using server.Models;
using server.Models.DTO;

namespace server.Profiles
{
    public class GiftProfile : Profile
    {
        public GiftProfile()
        {

            CreateMap<GiftDTO, Gift>();
            CreateMap<Gift, GiftDTO>();
        }
    }
}
