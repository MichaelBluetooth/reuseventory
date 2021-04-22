using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using ReuseventoryApi.Authentication;
using ReuseventoryApi.Models;

namespace ReuseventoryApi.Controllers
{
    [Authorize]
    public class UsersController : ODataController
    {
        private readonly ReuseventoryDbContext _ctx;
        private readonly IJwtAuthManager _jwtAuthManager;
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(ReuseventoryDbContext ctx, ILogger<UsersController> logger, IUserService userService, IJwtAuthManager jwtAuthManager)
        {
            _ctx = ctx;
            _userService = userService;
            _jwtAuthManager = jwtAuthManager;
            _logger = logger;
        }

        [EnableQuery]
        public IQueryable<User> Get()
        {
            return _ctx.Users;
        }

        [EnableQuery]
        public SingleResult<User> Get([FromODataUri] Guid key)
        {
            IQueryable<User> result = _ctx.Users.Where(p => p.id == key);
            return SingleResult.Create(result);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Register(ODataActionParameters parameters)
        {
            LoginRequest request = new LoginRequest()
            {
                username = parameters["username"]?.ToString(),
                password = parameters["password"]?.ToString()
            };

            if (String.IsNullOrEmpty(request.username) || String.IsNullOrEmpty(request.password))
            {
                return BadRequest();
            }

            User user = _userService.findUserByUserName(request.username);
            if (null != user)
            {
                _logger.LogInformation($"Anonymous user tried to register with [{request.username}], but it was already registered.");
                return BadRequest("Username is already taken");
            }

            if (!_userService.isValidPassword(request.password))
            {
                return BadRequest("Passwords must have a minimum of 8 characters, at least one uppercase letter and at least one number");
            }

            _userService.Register(request.username, request.password);
            _ctx.SaveChanges();
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(ODataActionParameters parameters)
        {
            LoginRequest request = new LoginRequest()
            {
                username = parameters["username"]?.ToString(),
                password = parameters["password"]?.ToString()
            };

            if (String.IsNullOrEmpty(request.username) || String.IsNullOrEmpty(request.password))
            {
                return BadRequest();
            }

            if (!_userService.IsValidUserCredentials(request.username, request.password))
            {
                return Unauthorized();
            }

            User user = _userService.findUserByUserName(request.username);
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, request.username)
            };

            var jwtResult = _jwtAuthManager.GenerateTokens(request.username, claims, DateTime.Now);
            _logger.LogInformation($"User [{request.username}] logged in the system.");
            return Ok(new LoginResult
            {
                username = request.username,
                accessToken = jwtResult.accessToken,
                refreshToken = jwtResult.refreshToken.tokenString
            });
        }

        [HttpPost]
        public ActionResult Logout()
        {
            string userName = User.Identity.Name;
            _jwtAuthManager.RemoveRefreshTokenByUserName(userName);
            _logger.LogInformation($"User [{userName}] logged out the system.");
            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> RefreshToken(ODataActionParameters parameters)
        {
            RefreshTokenRequest request = new RefreshTokenRequest()
            {
                refreshToken = parameters["refreshToken"]?.ToString(),
            };

            try
            {
                var userName = User.Identity.Name;
                _logger.LogInformation($"User [{userName}] is trying to refresh JWT token.");

                if (string.IsNullOrWhiteSpace(request.refreshToken))
                {
                    return Unauthorized();
                }

                var accessToken = await HttpContext.GetTokenAsync("Bearer", "access_token");
                var jwtResult = _jwtAuthManager.Refresh(request.refreshToken, accessToken, DateTime.Now);
                _logger.LogInformation($"User [{userName}] has refreshed JWT token.");
                return Ok(new LoginResult
                {
                    username = userName,
                    accessToken = jwtResult.accessToken,
                    refreshToken = jwtResult.refreshToken.tokenString
                });
            }
            catch (SecurityTokenException e)
            {
                return Unauthorized(e.Message); // return 401 so that the client side can redirect the user to login page
            }
        }
    }
}