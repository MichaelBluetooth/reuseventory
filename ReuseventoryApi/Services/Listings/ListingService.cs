using System;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using ReuseventoryApi.Helpers;
using ReuseventoryApi.Models;
using ReuseventoryApi.Models.DTO;

namespace ReuseventoryApi.Services.Listings
{
    public class ListingsService : IListingsService
    {
        private readonly ReuseventoryDbContext _ctx;
        private readonly IMapper _mapper;

        public ListingsService(ReuseventoryDbContext ctx, IMapper mapper)
        {
            _ctx = ctx;
            _mapper = mapper;
        }

        public PagedResult<ListingDTO> searchListings(int pageSize = 100, int page = 1, string query = "")
        {
            if (!string.IsNullOrEmpty(query))
            {
                return _ctx.Listings
                            .Include(l => l.user)
                            .Include(l => l.tags)
                            .Where(l => l.name.ToLower().Contains(query.ToLower()) || l.tags.Any(t => t.name.ToLower().Contains(query.ToLower())))
                            .ProjectTo<ListingDTO>(_mapper.ConfigurationProvider).GetPaged(page, pageSize);
            }
            else
            {
                return _ctx.Listings.ProjectTo<ListingDTO>(_mapper.ConfigurationProvider).GetPaged(page, pageSize);
            }
        }

        public ListingDTO getListing(Guid key)
        {
            Listing result = _ctx.Listings
                .Include(l => l.user)
                .Include(l => l.tags)
                .Where(l => l.id == key)
                .FirstOrDefault();
            return _mapper.Map<ListingDTO>(result);
        }
    }
}