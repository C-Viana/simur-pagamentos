using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using simur_backend.Models.DTO.V1;
using simur_backend.Services.Users;

namespace simur_backend.Controllers.Authorization
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserServices _service;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserServices userService, ILogger<AuthController> logger)
        {
            _service = userService;
            _logger = logger;
        }

        [HttpPost("create", Name = "CreateUser")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        public async Task<IActionResult> CreateUser([FromBody] UserDto subscribingUser)
        {
            _logger.LogInformation("User creation request for {username}", subscribingUser.Username);
            if (string.IsNullOrEmpty(subscribingUser.Username) || string.IsNullOrEmpty(subscribingUser.Password))
                return BadRequest("Username and password must not be null or empty");
            if (_service.UserExists(subscribingUser.Username, subscribingUser.Email))
                return BadRequest("Username and/or e-mail already in use");

            UserDto createdUser = await _service.CreateUserAsync(subscribingUser);
            return CreatedAtAction(null, new { createdUser.Id }, createdUser);
        }

        [HttpPost("signin", Name = "Signin")]
        [ProducesResponseType(typeof(UserCredentialsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        public async Task<IActionResult> Signin([FromBody] UserCredentialsDto credentials)
        {
            _logger.LogInformation("Authentication request for user {username}", credentials.Username);
            if (string.IsNullOrEmpty(credentials.Username) || string.IsNullOrEmpty(credentials.Password))
                return BadRequest("Username and password must not be null or empty");
            UserTokenDto token = await _service.AuthenticadeUserAsync(credentials);
            if (token is null) return BadRequest("Authentication failed. Review credentials and try again");
            return Ok(token);
        }

        [HttpPost("refresh", Name = "RefreshToken")]
        [ProducesResponseType(typeof(UserCredentialsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] UserTokenDto token)
        {
            _logger.LogInformation("Access renewal request");
            if (string.IsNullOrEmpty(token.AccessToken) || string.IsNullOrEmpty(token.RefreshToken))
                return BadRequest("Access token and refresh token must not be null or empty");
            UserTokenDto renewedToken = await _service.ValidateCredentialsAsync(token);
            if (renewedToken is null) return Unauthorized("Refresh token failed. Token is either invalid or expired. Try to sign in again");
            _logger.LogInformation("Access successfully renewed");
            return Ok(renewedToken);
        }

        [HttpPost("revoke", Name = "RevokeToken")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        public async Task<IActionResult> RevokeToken()
        {
            var username = User.Identity.Name;
            if (string.IsNullOrEmpty(username)) return BadRequest("Username is missing");

            _logger.LogInformation("Logging out user {username}", username);
            var result = await _service.RevokeTokenAsync(username);
            if(!result) return BadRequest("Failed to revoke token");

            _logger.LogInformation("User {username} session has been closed", username);
            return NoContent();
        }
    }
}
