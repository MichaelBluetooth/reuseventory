using System;
using Microsoft.AspNetCore.OData.Deltas;
using ReuseventoryApi.Models;
using ReuseventoryApi.Models.DTO;

namespace ReuseventoryApi.Authentication
{
    public interface IUserService
    {
        bool IsValidUserCredentials(string username, string password);
        User findUserByUserName(string username);
        User Register(string username, string password);
        bool isValidPassword(string password);
        void update(Guid key, Delta<User> changes);
        bool isValidUpdate(Guid key);
    }
}