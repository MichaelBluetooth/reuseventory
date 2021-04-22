using System.Linq;
using Microsoft.AspNetCore.Http;
using ReuseventoryApi.Models;

namespace ReuseventoryApi.Services.CurrentUser
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly ReuseventoryDbContext _ctx;
        private readonly IHttpContextAccessor _accessor;

        public CurrentUserService(ReuseventoryDbContext ctx, IHttpContextAccessor accessor)
        {
            _ctx = ctx;
            _accessor = accessor;
        }

        public User GetCurrentUser()
        {
            return _ctx.Users.FirstOrDefault(u => u.username == _accessor.HttpContext.User.Identity.Name);
        }
    }
}