using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly IListingsService _listingService;

        public ListingsController(ReuseventoryDbContext ctx, ILogger<ListingsController> logger, IListingsService listingService)
        {
            _ctx = ctx;
            _logger = logger;
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
            if (!Exists(key))
            {
                return NotFound();                
            }

            return Ok(_listingService.getListing(key));
        }

        [HttpDelete]
        [Route("{key}")]
        [Authorize(Policy = "IsOwnerOrAdmin")]
        public ActionResult Delete(Guid key)
        {
            if (!Exists(key))
            {
                return NotFound();
            }

            _listingService.deleteListing(key);

            return NoContent();
        }

        [HttpPost]
        public IActionResult Post([FromBody] ListingCreateOrUpdate listingDTO)
        {
            return Ok(_listingService.createListing(listingDTO));
        }

        [HttpPut]
        [Route("{key}")]
        [Authorize(Policy = "IsOwnerOrAdmin")]
        public IActionResult Put(Guid key, [FromBody] ListingCreateOrUpdate update)
        {
            if (!Exists(key))
            {
                return NotFound();
            }

            return Ok(_listingService.updateListing(key, update));
        }
    }
}
