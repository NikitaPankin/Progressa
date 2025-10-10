using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace Backend.Controllers
{
    public class LoginRequest
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class SessionController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ISessionRepository _sessionRepository;

        public SessionController(IUserRepository userRepository, ISessionRepository sessionRepository)
        {
            _userRepository = userRepository;
            _sessionRepository = sessionRepository;
        }

        [HttpPost("signin")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Login) || string.IsNullOrWhiteSpace(request.Password)) return BadRequest("Login and password are required.");

            var user = await _userRepository.GetUserByLoginAsync(request.Login);
            if (user == null) return NotFound("User not found, please register.");

            if (!await _userRepository.CheckPasswordAsync(request.Login, request.Password)) return Unauthorized("Invalid password.");

            var tokens = await _sessionRepository.CreateAccessRefreshTokenAsync(user);

            var handler = new JwtSecurityTokenHandler();
            return Ok(new
            {
                AccessToken = handler.WriteToken(tokens.accessToken),
                RefreshToken = tokens.refreshToken
            });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken)) return BadRequest("Refresh token is required.");

            try
            {
                var newTokenPair = await _sessionRepository.ExtendSessionByRefreshTokenAsync(refreshToken);

                var handler = new JwtSecurityTokenHandler();
                return Ok(new
                {
                    AccessToken = handler.WriteToken(newTokenPair.accessToken),
                    RefreshToken = newTokenPair.refreshToken
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }            
        }
    }
}
