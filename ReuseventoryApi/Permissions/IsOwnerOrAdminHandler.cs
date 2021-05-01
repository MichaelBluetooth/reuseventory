using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ReuseventoryApi.Models;

namespace ReuseventoryApi.Permissions
{
    public class IsOwnerOrAdminHandler : AuthorizationHandler<IsOwnerOrAdminRequirement>, IAuthorizationRequirement
    {
        private readonly ReuseventoryDbContext _ctx;
        private readonly ILogger<IsOwnerOrAdminHandler> _logger;

        public IsOwnerOrAdminHandler(ReuseventoryDbContext ctx, ILogger<IsOwnerOrAdminHandler> logger)
        {
            _ctx = ctx;
            _logger = logger;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsOwnerOrAdminRequirement requirement)
        {
            User currentUser = _ctx.Users.FirstOrDefault(u => u.username.Equals(context.User.Identity.Name, StringComparison.CurrentCultureIgnoreCase));
            if (context.Resource is DefaultHttpContext httpContext)
            {
                RouteValueDictionary routeData = httpContext.Request.RouteValues;

                if (routeData.ContainsKey("key"))
                {
                    string guidRaw = routeData.First(kv => kv.Key.Equals("key")).Value.ToString();
                    Guid key = Guid.Parse(guidRaw);
                    Listing listing = _ctx.Listings.AsNoTracking().FirstOrDefault(p => p.id == key);
                    if (listing != null && requirement.isOwnerOrAdmin(currentUser, listing))
                    {
                        context.Succeed(requirement);
                    }
                    else
                    {
                        string verb = httpContext.Request.Method;
                        _logger.LogInformation($"User ${currentUser.username} ({currentUser.id}) attempted to {verb} listing {listing.id} but was not the owner or admin");
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}