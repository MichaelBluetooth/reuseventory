using System;
using ReuseventoryApi.Models.DTO;

namespace ReuseventoryApi.Services.Listings
{
    public interface IListingsService
    {
        PagedResult<ListingDTO> searchListings(int pageSize = 100, int page = 1, string query = "", Guid? owner = null);

        ListingDTO getListing(Guid key);

        ListingDTO createListing(ListingCreateOrUpdate listingDTO);

        ListingDTO updateListing(Guid key, ListingCreateOrUpdate update);

        void deleteListing(Guid key);
    }
}