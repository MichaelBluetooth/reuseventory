using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ReuseventoryApi.Models;

namespace ReuseventoryApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class ListingImagesController : ControllerBase
    {
        private readonly ReuseventoryDbContext _ctx;
        private readonly ILogger<ListingImagesController> _logger;

        public ListingImagesController(ReuseventoryDbContext ctx, ILogger<ListingImagesController> logger)
        {
            _ctx = ctx;
            _logger = logger;
        }

        [HttpGet]
        [Route("{listingId}")]
        public ActionResult Get(Guid listingId)
        {
            ListingImage img = _ctx.ListingImages.FirstOrDefault(li => li.listingId == listingId);
            if (null == img)
            {
                return NotFound();
            }
            else
            {
                return File(img.image, img.contentType);
            }
        }
    }
}