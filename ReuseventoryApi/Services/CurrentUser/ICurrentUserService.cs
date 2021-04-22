using ReuseventoryApi.Models;

namespace ReuseventoryApi.Services.CurrentUser
{
    public interface ICurrentUserService
    {
        User GetCurrentUser();
    }
}