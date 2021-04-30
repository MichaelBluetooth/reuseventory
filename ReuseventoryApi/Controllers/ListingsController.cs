using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ReuseventoryApi.Helpers;
using ReuseventoryApi.Models;
using ReuseventoryApi.Models.DTO;
using ReuseventoryApi.Services.Listings;

namespace ReuseventoryApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class ListingsController : ControllerBase
    {
        private readonly ReuseventoryDbContext _ctx;
        private readonly ILogger<ListingsController> _logger;
        private readonly IMapper _mapper;
        private readonly ListingsService _listingService;

        public ListingsController(ReuseventoryDbContext ctx, ILogger<ListingsController> logger, IMapper mapper, ListingsService listingService)
        {
            _ctx = ctx;
            _logger = logger;
            _mapper = mapper;
            _listingService = listingService;

            if (_ctx.Listings.Count() == 0)
            {
                for (int user = 0; user < 20; user++)
                {
                    _ctx.Users.Add(new User()
                    {
                        username = "test_user_" + user,
                        password = "password",
                        phone = DateTime.UtcNow.Ticks.ToString().Substring(8),
                        email = "test_user_" + user + "@gmail.com",
                        isAdmin = false
                    });
                }
                _ctx.SaveChanges();

                Random r = new Random(DateTime.Now.Millisecond);
                int count = _ctx.Users.Count();
                for (int listing = 0; listing < 100; listing++)
                {
                    ICollection<ListingTag> tags = new List<ListingTag>();
                    int numTags = r.Next(5);
                    for (int t = 0; t < numTags; t++)
                    {
                        tags.Add(
                            new ListingTag()
                            {
                                name = "tag_" + r.Next(35)
                            }
                        );
                    }

                    _ctx.Listings.Add(new Listing()
                    {
                        name = "listing_" + listing,
                        userId = _ctx.Users.ToList()[r.Next(count)].id,
                        description = "This is a brief description of the item.",
                        tags = tags
                    });
                }

                _ctx.SaveChanges();
            }
        }

        private bool Exists(Guid key)
        {
            return _ctx.Listings.Any(p => p.id == key);
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult<PagedResult<ListingDTO>> Get([FromQuery] int pageSize = 100, [FromQuery] int page = 1, [FromQuery] string q = "")
        {
            return Ok(_listingService.searchListings(pageSize, page, q));
        }

        [HttpGet]
        [Route("{key}")]
        [AllowAnonymous]
        public ActionResult<ListingDTO> Get(Guid key)
        {
            if (Exists(key))
            {
                return Ok(_listingService.getListing(key));
            }
            else
            {
                return NotFound();
            }
        }

        [HttpDelete]
        [Route("{key}")]
        public ActionResult Delete(Guid key)
        {
            Listing doomed = _ctx.Listings.FirstOrDefault(p => p.id == key);
            if (null == doomed)
            {
                return NotFound();
            }

            User currentUser = _ctx.Users.FirstOrDefault(u => u.username == User.Identity.Name);
            if (doomed.userId == currentUser.id || currentUser.isAdmin)
            {
                _ctx.Listings.Remove(doomed);
                _ctx.SaveChanges();
            }
            else
            {
                _logger.LogInformation($"User {currentUser.username} tried to delete Listing {key}, but did own own the listing or was not an administrator.");
                return BadRequest("Users may only delete their own listings, or must be an administrator");
            }

            return NoContent();
        }

        [HttpPost]
        public IActionResult Post([FromBody] ListingCreateOrUpdate listingDTO)
        {
            Listing listing = new Listing()
            {
                name = listingDTO.name,
                description = listingDTO.description,
                created = DateTime.UtcNow,
                modified = DateTime.UtcNow
            };
            User currentUser = _ctx.Users.FirstOrDefault(u => u.username == User.Identity.Name);
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
            return Ok(_mapper.Map<ListingDTO>(result));
        }

        [HttpPut]
        [Route("{key}")]
        public IActionResult Put([FromODataUri] Guid key, [FromBody] ListingCreateOrUpdate update)
        {
            Listing original = _ctx.Listings.AsNoTracking().FirstOrDefault(p => p.id == key);
            if (!Exists(key))
            {
                return NotFound();
            }

            User currentUser = _ctx.Users.FirstOrDefault(u => u.username == User.Identity.Name);
            if (original.userId == currentUser.id || currentUser.isAdmin)
            {
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

                original.name = update.name;
                original.description = update.description;
                original.modified = DateTime.UtcNow;
                _ctx.Entry(original).State = EntityState.Modified;
                _ctx.SaveChanges();
            }
            else
            {
                _logger.LogInformation($"User {currentUser.username} tried to update Listing {key}, but did own own the listing or was not an administrator.");
                return BadRequest("Users may only edit their own listings, or must be an administrator");
            }

            Listing result = _ctx.Listings
                    .Include(l => l.user)
                    .Include(l => l.tags)
                    .Where(l => l.id == key)
                    .FirstOrDefault();
            return Ok(_mapper.Map<ListingDTO>(result));
        }
    }
}
