using Microsoft.AspNetCore.Authorization;
using ReuseventoryApi.Models;

namespace ReuseventoryApi.Permissions
{
    public class IsOwnerOrAdminRequirement: IAuthorizationRequirement
    {
        public bool isOwnerOrAdmin(User currentUser, Listing listing)
        {
            return currentUser.isAdmin || listing.userId == currentUser.id;
        }
    }
}