using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace Backend.Controllers
{
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
        public async Task<IActionResult> Login([FromHeader] string login, [FromHeader] string password)
        {
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password)) return BadRequest("Login and password are required.");

            var user = await _userRepository.GetUserByLoginAsync(login);
            if (user == null) return NotFound("User not found, please register.");

            if (!await _userRepository.CheckPasswordAsync(login, password)) return Unauthorized("Invalid password.");

            var tokens = await _sessionRepository.CreateAccessRefreshTokenAsync(user);

            var handler = new JwtSecurityTokenHandler();
            return Ok(new
            {
                AccessToken = handler.WriteToken(tokens.accessToken),
                RefreshToken = handler.WriteToken(tokens.refreshToken)
            });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromHeader] string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken)) BadRequest("Refresh token is required.");

            try
            {
                var newTokenPair = await _sessionRepository.ExtendSessionByRefreshTokenAsync(refreshToken);

                var handler = new JwtSecurityTokenHandler();
                return Ok(new
                {
                    AccessToken = handler.WriteToken(newTokenPair.accessToken),
                    RefreshToken = handler.WriteToken(newTokenPair.refreshToken)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }            
        }
    }
}
