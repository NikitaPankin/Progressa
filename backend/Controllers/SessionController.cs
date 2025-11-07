using Application.Interfaces;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Net;

namespace Backend.Controllers
{
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class RegisterRequest
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class SessionController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ISessionRepository _sessionRepository;
        private readonly IEmailSender _emailSender;
        private readonly IEmailTemplateService _emailTemplateService;

        public SessionController(IUserRepository userRepository, ISessionRepository sessionRepository, IEmailSender emailSender, IEmailTemplateService emailTemplateService)
        {
            _userRepository = userRepository;
            _sessionRepository = sessionRepository;
            _emailSender = emailSender;
            _emailTemplateService = emailTemplateService;
        }

        [HttpPost("signin")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password)) return BadRequest("Email and password are required.");

            var user = await _userRepository.GetUserByEmailAsync(request.Email);
            if (user == null) return NotFound("User not found, please register.");

            if (!await _userRepository.CheckPasswordAsync(request.Email, request.Password)) return Unauthorized("Invalid password.");
            if (!user.EmailConfirmed) return BadRequest("Email not confirmed. Please check your email.");

            var tokens = await _sessionRepository.CreateAccessRefreshTokenAsync(user);

            var handler = new JwtSecurityTokenHandler();
            return Ok(new
            {
                AccessToken = handler.WriteToken(tokens.accessToken),
                RefreshToken = tokens.refreshToken
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name)
                || string.IsNullOrWhiteSpace(request.Password)
                || string.IsNullOrWhiteSpace(request.Email))
                return BadRequest("Email, Login and password are required.");

            var existingUser = await _userRepository.GetUserByEmailAsync(request.Email);
            if (existingUser != null) return Conflict("A user with this email already exists.");

            try
            {
                var user = await _userRepository.CreateUserAsync(request.Email, request.Name, request.Password);
                var token = await _userRepository.GenerateEmailConfirmationTokenAsync(user);
                var confirmationUrl = Url.Action(
                    "ConfirmEmail",
                    "Session",
                    new { userId = user.Id, token },
                    Request.Scheme
                );

                var html = _emailTemplateService.GetEmailConfirmationTemplate(user.FullName, confirmationUrl);
                await _emailSender.SendEmailAsync(user.Email, "Email confirmation", html);

                return StatusCode(201, new { status = "ok", requiresEmailConfirmation = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("resend-confirmation")]
        public async Task<IActionResult> ResendConfirmation([FromBody] string email)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null) return NotFound();

            if (user.EmailConfirmed) return BadRequest("Email already confirmed");

            try
            {
                var token = await _userRepository.GenerateEmailConfirmationTokenAsync(user);
                var confirmationUrl = Url.Action(
                    "ConfirmEmail",
                    "Session",
                    new { userId = user.Id, token },
                    Request.Scheme
                );

                var html = _emailTemplateService.GetEmailConfirmationTemplate(user.FullName, confirmationUrl);
                await _emailSender.SendEmailAsync(user.Email, "Email confirmation", html);

                return StatusCode(201, new { status = "ok", requiresEmailConfirmation = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null) return Redirect("http://localhost:5173/register?error=invalid_token");

            var result = await _userRepository.ConfirmEmailAsync(user, token);
            if (result) return Redirect("http://localhost:5173/signin");

            return Redirect("http://localhost:5173/register?error=invalid_token");
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
