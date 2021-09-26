using System;
using System.IO;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ReuseventoryApi.Helpers;
using ReuseventoryApi.Models;
using ReuseventoryApi.Models.DTO;
using ReuseventoryApi.Services.CurrentUser;

namespace ReuseventoryApi.Services.Listings
{
    public class ListingsService : IListingsService
    {
        private readonly ReuseventoryDbContext _ctx;
        private readonly IMapper _mapper;
        private readonly ILogger<ListingsService> _logger;
        private readonly ICurrentUserService _currentUserService;

        public ListingsService(ReuseventoryDbContext ctx, IMapper mapper, ICurrentUserService currentUserService, ILogger<ListingsService> logger)
        {
            _ctx = ctx;
            _mapper = mapper;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public PagedResult<ListingDTO> searchListings(int pageSize = 100, int page = 1, string query = "", Guid? owner = null)
        {
            var dbQuery = _ctx.Listings
                            .OrderBy(l => l.name)
                            .Include(l => l.user)
                            .Include(l => l.tags)
                            .AsQueryable();

            if (!string.IsNullOrEmpty(query))
            {
                dbQuery = dbQuery.Where(l => l.name.ToLower().Contains(query.ToLower()) || l.tags.Any(t => t.name.ToLower().Contains(query.ToLower()))).AsQueryable();
            }

            if(null != owner){
                dbQuery = dbQuery.Where(l => l.userId == owner).AsQueryable();
            }

            return dbQuery.ProjectTo<ListingDTO>(_mapper.ConfigurationProvider).GetPaged(page, pageSize);
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

        public ListingDTO createListing(ListingCreateOrUpdate listingDTO)
        {
            Listing listing = new Listing()
            {
                name = listingDTO.name,
                description = listingDTO.description,
                created = DateTime.UtcNow,
                modified = DateTime.UtcNow,
                price = listingDTO.price
            };

            User currentUser = _currentUserService.GetCurrentUser();
            listing.userId = currentUser.id;
            Guid key = _ctx.Listings.Add(listing).Entity.id.Value;
            foreach (string tag in listingDTO.tags)
            {
                _ctx.ListingTags.Add(new ListingTag()
                {
                    name = tag,
                    listingId = key
                });
            }

            if (null != listingDTO.image && FileHelper.IsImage(listingDTO.image))
            {
                byte[] fileContents = null;
                using (var fs1 = listingDTO.image.OpenReadStream())
                using (var ms1 = new MemoryStream())
                {
                    fs1.CopyTo(ms1);
                    fileContents = ms1.ToArray();
                }

                _ctx.ListingImages.Add(new ListingImage()
                {
                    listingId = key,
                    contentType = listingDTO.image.ContentType,
                    image = fileContents,
                    fileName = Path.GetFileName(listingDTO.image.FileName)
                });
            }

            _ctx.SaveChanges();

            Listing result = _ctx.Listings
                    .Include(l => l.user)
                    .Include(l => l.tags)
                    .Where(l => l.id == key)
                    .FirstOrDefault();

            return _mapper.Map<ListingDTO>(result);
        }

        public ListingDTO updateListing(Guid key, ListingCreateOrUpdate update)
        {
            Listing original = _ctx.Listings.AsNoTracking().FirstOrDefault(p => p.id == key);
            User currentUser = _currentUserService.GetCurrentUser();
            if (null != update.tags)
            {
                foreach (ListingTag doomedTag in _ctx.ListingTags.Where(t => t.listingId == key))
                {
                    _ctx.ListingTags.Remove(doomedTag);
                }

                foreach (string tag in update.tags)
                {
                    _ctx.ListingTags.Add(new ListingTag()
                    {
                        name = tag,
                        listingId = key
                    });
                }
            }

            if (null != update.image && FileHelper.IsImage(update.image))
            {
                ListingImage existing = _ctx.ListingImages.FirstOrDefault(li => li.listingId == key);
                if (null != existing)
                {
                    _ctx.ListingImages.Remove(existing);
                }

                byte[] fileContents = null;
                using (var fs1 = update.image.OpenReadStream())
                using (var ms1 = new MemoryStream())
                {
                    fs1.CopyTo(ms1);
                    fileContents = ms1.ToArray();
                }

                _ctx.ListingImages.Add(new ListingImage()
                {
                    listingId = key,
                    contentType = update.image.ContentType,
                    image = fileContents,
                    fileName = Path.GetFileName(update.image.FileName)
                });
            }

            original.name = update.name;
            original.description = update.description;
            original.modified = DateTime.UtcNow;
            original.price = update.price;
            _ctx.Entry(original).State = EntityState.Modified;
            _ctx.SaveChanges();

            Listing result = _ctx.Listings
                    .Include(l => l.user)
                    .Include(l => l.tags)
                    .Where(l => l.id == key)
                    .FirstOrDefault();
            return _mapper.Map<ListingDTO>(result);
        }

        public void deleteListing(Guid key)
        {
            Listing doomed = _ctx.Listings.FirstOrDefault(p => p.id == key);
            if (null != doomed)
            {
                _ctx.Listings.Remove(doomed);
                _ctx.SaveChanges();
                _logger.LogInformation($"User {_currentUserService.GetCurrentUser().username} deleted listing {key}");
            }
        }
    }
}