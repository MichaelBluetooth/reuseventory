using System;
using System.Linq;
using System.Text.RegularExpressions;
using AutoMapper;
using Microsoft.AspNetCore.OData.Deltas;
using ReuseventoryApi.Models;
using ReuseventoryApi.Models.DTO;
using ReuseventoryApi.Services.CurrentUser;

namespace ReuseventoryApi.Authentication
{
    public class UserService : IUserService
    {
        private readonly ReuseventoryDbContext _ctx;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public UserService(ReuseventoryDbContext ctx, IMapper mapper, ICurrentUserService currentUserService)
        {
            _ctx = ctx;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public bool IsValidUserCredentials(string username, string password)
        {
            bool isValid = false;

            if (!string.IsNullOrWhiteSpace(password) && !string.IsNullOrEmpty(username))
            {
                User user = findUserByUserName(username);
                if (null != user)
                {
                    isValid = BCrypt.Net.BCrypt.Verify(password, user.password);
                }
            }

            return isValid;
        }

        public User findUserByUserName(string username)
        {
            return _ctx.Users.FirstOrDefault(u => u.username.ToLower() == username.ToLower());
        }

        public User Register(string username, string password)
        {
            User newUser = new User()
            {
                username = username,
                password = BCrypt.Net.BCrypt.HashPassword(password),
            };
            _ctx.Users.Add(newUser);

            return newUser;
        }

        public bool isValidPassword(string password)
        {
            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasMinimum8Chars = new Regex(@".{8,}");

            return hasUpperChar.IsMatch(password) &&
                   hasNumber.IsMatch(password) &&
                   hasMinimum8Chars.IsMatch(password);
        }

        public void update(Guid key, UserUpdate changes)
        {
            User user = _ctx.Users.Find(key);
            user = _mapper.Map(changes, user);
            if (null != changes.password && isValidPassword(changes.password))
            {
                user.password = BCrypt.Net.BCrypt.HashPassword(changes.password);
            }
            _ctx.Users.Update(user);
        }

        public bool isValidUpdate(Guid key)
        {
            User currentUser = _currentUserService.GetCurrentUser();
            return currentUser.isAdmin || currentUser.id == key;
        }
    }
}