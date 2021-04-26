using System;
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
        void update(Guid key, UserUpdate changes);
        bool isValidUpdate(Guid key);
    }
}