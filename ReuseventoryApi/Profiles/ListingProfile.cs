using System.Linq;
using AutoMapper;
using ReuseventoryApi.Models;
using ReuseventoryApi.Models.DTO;

namespace ReuseventoryApi.Profiles
{
    public class ListingProfile : Profile
    {
        public ListingProfile()
        {
            CreateMap<ListingCreateOrUpdate, Listing>();

            CreateMap<Listing, ListingDTO>()
                .ForMember(dest => dest.hasImage, opt => opt.MapFrom(so => so.images.Any()))
                .ForMember(dest => dest.tags, opt => opt.MapFrom(so => so.tags.Select(t => t.name).ToList())); ;
        }
    }
}