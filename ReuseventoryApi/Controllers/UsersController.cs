using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using ReuseventoryApi.Authentication;
using ReuseventoryApi.Helpers;
using ReuseventoryApi.Models;
using ReuseventoryApi.Models.DTO;

namespace ReuseventoryApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ReuseventoryDbContext _ctx;
        private readonly IJwtAuthManager _jwtAuthManager;
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;
        private readonly IMapper _mapper;

        public UsersController(ReuseventoryDbContext ctx, ILogger<UsersController> logger, IUserService userService, IJwtAuthManager jwtAuthManager, IMapper mapper)
        {
            _ctx = ctx;
            _userService = userService;
            _jwtAuthManager = jwtAuthManager;
            _logger = logger;
            _mapper = mapper;

            if (_ctx.Users.Count() == 0)
            {
                for (int i = 0; i < 250; i++)
                {
                    _ctx.Users.Add(new User()
                    {
                        username = new Guid().ToString()
                    });
                }
                _ctx.SaveChanges();
            }
        }

        private bool Exists(Guid key)
        {
            return _ctx.Users.Any(p => p.id == key);
        }

        [HttpGet]
        public ActionResult<PagedResult<User>> Get([FromQuery] int pageSize = 100, [FromQuery] int page = 0)
        {
            var results = _ctx.Users
                .ProjectTo<UserDTO>(_mapper.ConfigurationProvider)
                .GetPaged(page, pageSize);
            return Ok(results);
        }

        [HttpGet]
        [Route("{key}")]
        public ActionResult<User> Get(Guid key)
        {
            if (Exists(key))
            {
                User result = _ctx.Users.Find(key);
                return Ok(_mapper.Map<UserDTO>(result));
            }
            else
            {
                return NotFound();
            }
        }

        [HttpDelete]
        [Route("{key}")]
        public ActionResult Delete(Guid key)
        {
            User doomed = _ctx.Users.FirstOrDefault(p => p.id == key);
            if (null == doomed)
            {
                return NotFound();
            }

            User currentUser = _userService.findUserByUserName(User.Identity.Name);
            if (doomed.username == currentUser.username || currentUser.isAdmin)
            {
                _jwtAuthManager.RemoveRefreshTokenByUserName(doomed.username);
                _ctx.Users.Remove(doomed);
                _ctx.SaveChanges();
            }
            else
            {
                return BadRequest("Users may only delete their own account, or must be an administrator");
            }

            return NoContent();
        }

        [HttpPut]
        [Route("{key}")]
        public ActionResult Put([FromRoute] Guid key, [FromBody] UserUpdate changes)
        {
            if (false == ModelState.IsValid)
            {
                return BadRequest("Model state invalid");
            }

            if (false == _userService.isValidUpdate(key))
            {
                return BadRequest("Users may only edit their own profiles or be an administrator");
            }
            if (Exists(key))
            {
                _userService.update(key, changes);
                _ctx.SaveChanges();
            }
            else
            {
                return NotFound();
            }

            return Ok(_ctx.Users.Find(key));
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("register")]
        public ActionResult Register([FromBody] LoginRequest request)
        {
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

            User registered = _userService.Register(request.username, request.password);
            _ctx.SaveChanges();

            return Ok(_mapper.Map<UserDTO>(registered));
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public ActionResult Login([FromBody] LoginRequest request)
        {
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
        [Route("logout")]
        public ActionResult Logout()
        {
            string userName = User.Identity.Name;
            _jwtAuthManager.RemoveRefreshTokenByUserName(userName);
            _logger.LogInformation($"User [{userName}] logged out the system.");
            return Ok();
        }

        [HttpPost]
        [Route("refresh-token")]
        public async Task<ActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
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