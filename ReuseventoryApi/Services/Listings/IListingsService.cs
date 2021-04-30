using System;
using ReuseventoryApi.Models.DTO;

namespace ReuseventoryApi.Services.Listings
{
    public interface IListingsService
    {
        PagedResult<ListingDTO> searchListings(int pageSize = 100, int page = 1, string query = "");

        ListingDTO getListing(Guid key);
    }
}