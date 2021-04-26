using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ReuseventoryApi.Models;

namespace ReuseventoryApi.Controllers
{   
    [Authorize]
    [Route("api/[controller]")]
    public class ListingTagsController : ControllerBase
    {
        private readonly ReuseventoryDbContext _ctx;
        private readonly ILogger<ListingTagsController> _logger;

        public ListingTagsController(ReuseventoryDbContext ctx, ILogger<ListingTagsController> logger)
        {
            _ctx = ctx;
            _logger = logger;
        }

        [HttpGet]
        [Route("distinct")]
        [AllowAnonymous]
        public ActionResult Distinct()
        {
            return Ok(_ctx.ListingTags.Select(t => t.name).Distinct().ToList());
        }
    }
}
