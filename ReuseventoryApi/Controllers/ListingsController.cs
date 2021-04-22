using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using ReuseventoryApi.Authentication;
using ReuseventoryApi.Models;
using ReuseventoryApi.Models.DTO;

namespace ReuseventoryApi.Controllers
{
    [Authorize]
    public class ListingsController : ODataController
    {
        private readonly ReuseventoryDbContext _ctx;
        private readonly IJwtAuthManager _jwtAuthManager;
        private readonly ILogger<ListingsController> _logger;
        private readonly IMapper _mapper;

        public ListingsController(ReuseventoryDbContext ctx, ILogger<ListingsController> logger, IJwtAuthManager jwtAuthManager, IMapper mapper)
        {
            _ctx = ctx;
            _jwtAuthManager = jwtAuthManager;
            _logger = logger;
            _mapper = mapper;
        }

        private bool Exists(Guid key)
        {
            return _ctx.Listings.Any(p => p.id == key);
        }

        [EnableQuery]
        public SingleResult<Listing> Get([FromODataUri] Guid key)
        {
            IQueryable<Listing> result = _ctx.Listings.Where(p => p.id == key);
            return SingleResult.Create(result);
        }

        [EnableQuery]
        public ActionResult Delete([FromODataUri] Guid key)
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
                return BadRequest("Users may only delete their own listings, or must be an administrator");
            }

            return NoContent();
        }

        [EnableQuery]
        [HttpPost]
        public SingleResult<Listing> Post([FromBody] Listing listing)
        {
            User currentUser = _ctx.Users.FirstOrDefault(u => u.username == User.Identity.Name);
            listing.userId = currentUser.id;
            Guid key = _ctx.Listings.Add(listing).Entity.id.Value;
            _ctx.SaveChanges();

            IQueryable<Listing> result = _ctx.Listings.Where(p => p.id == key);
            return SingleResult.Create(result);
        }

        [EnableQuery]
        [HttpPut]
        public ActionResult Put([FromODataUri] Guid key, [FromBody] Listing update)
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

                    foreach (ListingTag tag in update.tags)
                    {
                        tag.listingId = key;
                        _ctx.ListingTags.Add(tag);
                    }
                }

                update.userId = original.userId;
                update.id = key;
                _ctx.Entry(update).State = EntityState.Modified;
                _ctx.SaveChanges();
            }
            else
            {
                return BadRequest("Users may only edit their own listings, or must be an administrator");
            }

            IQueryable<Listing> result = _ctx.Listings.Where(p => p.id == key);
            return Ok(SingleResult.Create(result));
        }

        [EnableQuery]
        [HttpPatch]
        public ActionResult Patch([FromODataUri] Guid key, [FromBody] Delta<Listing> changes)
        {
            Listing original = _ctx.Listings.FirstOrDefault(p => p.id == key);
            if (null == original)
            {
                return NotFound();
            }

            User currentUser = _ctx.Users.FirstOrDefault(u => u.username == User.Identity.Name);
            if (original.userId == currentUser.id || currentUser.isAdmin)
            {
                object tagsRaw;
                bool tagsFound = changes.TryGetPropertyValue("tags", out tagsRaw);
                if (tagsFound && null != tagsRaw)
                {
                    foreach (ListingTag doomedTag in _ctx.ListingTags.Where(t => t.id == key))
                    {
                        _ctx.ListingTags.Remove(doomedTag);
                    }

                    ICollection<ListingTag> tags = tagsRaw as List<ListingTag>;
                    foreach (ListingTag tag in tags.Where(t => t.id != null))
                    {
                        tag.listingId = key;
                        _ctx.ListingTags.Add(tag);
                    }
                }

                changes.Patch(original);
                _ctx.Listings.Update(original);
                _ctx.SaveChanges();
            }
            else
            {
                return BadRequest("Users may only edit their own listings, or must be an administrator");
            }

            IQueryable<Listing> result = _ctx.Listings.Where(p => p.id == key);
            return Ok(SingleResult.Create(result));
        }

    }
}
