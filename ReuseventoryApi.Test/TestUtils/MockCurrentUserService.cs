using System;
using ReuseventoryApi.Models;
using ReuseventoryApi.Services.CurrentUser;

namespace ReuseventoryApi.Test.TestUtils
{
    public class MockCurrentUserService : ICurrentUserService
    {
        private readonly User _user;

        public MockCurrentUserService(User user = null)
        {
            _user = user ?? new User() { username = "Default_Mock_User", id = Guid.NewGuid(), isAdmin = false };
        }

        public User GetCurrentUser()
        {
            return _user;
        }
    }
}